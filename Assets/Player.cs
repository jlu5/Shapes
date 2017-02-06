using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    // Public (editor-exposed) variables
    public float moveSpeed = 10;
    public float jumpStrength = 700;
    public float rotationSpeed = 3;
    public float jumpRecoilStrength = 100;
    public int playerID = 0;

    // Jump / Rigidbody basics tracking
    private Rigidbody2D rb;
    private bool canJump = false;
    private Vector2 nextJumpVector;
    private Dictionary<GameObject, Vector2> collidingObjects = new Dictionary<GameObject, Vector2>();

    // Tracking for player attachments
    private bool isAttached = false;
    private List<GameObject> collidingPlayers = new List<GameObject>();

    // Track player objects and the joints binding them to one another.
    protected Dictionary<GameObject, RelativeJoint2D> playerBinds = new Dictionary<GameObject, RelativeJoint2D>();

    // Tracks which player GameObject is leading the current player attachment (i.e. which player
    // has the RelativeJoint2D component).
    protected GameObject masterPlayer;

    // Use this for initialization
    void Start () {
        // Find our Rigidbody2D object
        rb = GetComponent<Rigidbody2D> ();
        if (playerID == 0)
        {
            Debug.LogWarning("Immovable player object with invalid ID!");
        }
        // Add this player to our global game state.
        GameState.Instance.addPlayer(playerID, this);

    }

    // Collision handler
    void OnCollisionEnter2D(Collision2D col)
    {
        // Objects with specific collision routines are (e.g. finishes) called here.
        // These each have a script component deriving from the Collidable class,
        // and override PlayerHit() to make this work.
        Collidable collidable = col.gameObject.GetComponent<Collidable>();
        Debug.Log(string.Format("Collidable = {0}", collidable));
        if (collidable != null)
        {
            collidable.PlayerHit(this);
            return; // XXX this is proabably not needed
        }

        // Collision with other players are tracked separately; this is used to process player
        // attachment.
        if (col.gameObject.CompareTag("Player")) {
            collidingPlayers.Add(col.gameObject);
        }

        // Track the objects we're currently colliding with along with the collision
        // angle. This is used to process jump.
        collidingObjects[col.gameObject] = col.contacts[0].normal;
            
        Debug.Log("can_jump set to true");
        canJump = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        collidingObjects.Remove(col.gameObject);

        if (collidingObjects.Count == 0)
        {
            // Disable jump when leaving all collisions, so that we can't magically gain momentum in mid air.
            Debug.Log("can_jump set to false");
            canJump = false;
        }

        if (col.gameObject.CompareTag("Player"))
        {
            collidingPlayers.Remove(col.gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        /* Make jump realistic: whenever the player collides with the environment, set the
            * jump direction perpendicular to the surface(s) the player is touching.
            * This is in contrast to making jump always point upwards, which is completely wrong
            * when colliding with slanted or floating platforms.
            * Since forces are expressed as a vector, we can sum all the collision normal
            * vectors if the player is touching multiple points at once (e.g. corner jump).
            */
        Vector2 normal_sum = Vector2.zero;

        foreach (Vector2 vector in collidingObjects.Values)
        {
            //Debug.Log(string.Format("Adding vector ({0}, {1}) from colliding object", vector.x, vector.y));
            normal_sum += vector;
        }
        nextJumpVector = normal_sum;
        // Normalize the resulting vector so we don't get super jumps when colliding with multiple
        // objects at the same time (e.g. when resting on the crack between two parallel platforms).
        nextJumpVector.Normalize();
    }

    // Update is called once per frame
    void Update () {
        // Get horizontal and vertical (rotation) movement

        if (GameState.Instance.currentPlayer == playerID)
        {
            float x_move = Input.GetAxis("Horizontal");
            float r_move = Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Jump") && canJump)
            {
                // Add a force on the player character to propel them perpendicular to the
                // surface(s) they're touching.
                rb.AddForce(nextJumpVector * jumpStrength);

                foreach (KeyValuePair<GameObject, Vector2> objpair in collidingObjects)
                {
                    // For each object that we're colliding with, exert an opposing force
                    // when we jump (if that object has a rigid body).
                    Rigidbody2D other_rb = objpair.Key.GetComponent<Rigidbody2D>();
                    if (other_rb != null)
                    {
                        other_rb.AddForce(-nextJumpVector * jumpRecoilStrength);
                    }
                }

                // Disable jump while we're in mid-air (this is reset in OnCollisionStay2D).
                canJump = false;
                Debug.Log("canJump set to false for player ID "+playerID);
            } else if (Input.GetButtonDown("Attach"))
            {
                // Attach was called.
                if (!isAttached)
                {
                    // If we're not already attached, create a relative joint binding
                    // the current player with all players it is colliding with.
                    isAttached = true;
                    foreach (GameObject playerObject in collidingPlayers)
                    {
                        RelativeJoint2D joint = gameObject.AddComponent<RelativeJoint2D>();
                        joint.connectedBody = playerObject.GetComponent<Rigidbody2D>();
                        playerBinds[playerObject] = joint;

                        // Don't track collisions between the player and other bounded ones;
                        joint.enableCollision = false;

                        // Track which player has the RelativeJoint2D, so that other players
                        // can unbind themselves.
                        Player otherPlayer = playerObject.GetComponent<Player>();
                        otherPlayer.masterPlayer = gameObject;
                        otherPlayer.isAttached = true;
                    }
                } else {
                    // Otherwise, delete all joints.
                    isAttached = false;
                    if (masterPlayer != null)
                    {
                        // If we're not the master player in an attachment, the relative
                        // joint we need to delete will be in another player object.
                        Dictionary<GameObject, RelativeJoint2D> masterPlayerBinds = masterPlayer.GetComponent<Player>().playerBinds;
                        Debug.Log(string.Format("Got masterPlayerBinds %s", masterPlayerBinds));
                        RelativeJoint2D joint = masterPlayerBinds[gameObject];
                        masterPlayerBinds.Remove(gameObject);
                        Destroy(joint);
                        masterPlayer = null;
                    } else
                    {
                        foreach (RelativeJoint2D joint in GetComponents<RelativeJoint2D>())
                        {
                            // Remove the old player binds.
                            playerBinds.Remove(joint.connectedBody.gameObject);
                            Destroy(joint);
                        }
                    }
                }
            }

            Vector2 vector_move = new Vector2(x_move, 0.0F);

            rb.AddForce(vector_move * moveSpeed);
            // Up rotates clockwise, down rotates counterclockwise
            rb.AddTorque(-r_move * rotationSpeed);
        }
    }

    // Handles mouse clicks on the player, which sets it to the current one.
    void OnMouseUp()
    {
        GameState.Instance.currentPlayer = playerID;
    }

    // Returns the color object of the player.
    public Color getColor()
    {
        return GetComponent<SpriteRenderer>().color;
    }
}