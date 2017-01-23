using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float moveSpeed = 10;
    public float jumpStrength = 700;
    public float rotationSpeed = 3;
    public float jumpRecoilStrength = 100;
    public int playerID = 0;

    private Rigidbody2D rb;
    private bool canJump = false;
    private Vector2 nextJumpVector;
    private Dictionary<GameObject, Vector2> collidingObjects = new Dictionary<GameObject, Vector2>();

    // Use this for initialization
    void Start () {
        // Find our Rigidbody2D object
        rb = GetComponent<Rigidbody2D> ();
        // Add this player to our global game state.
        GameState.Instance.addPlayer(playerID, this);

    }

    void OnCollisionEnter2D(Collision2D col)
    {
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
            normal_sum += vector;
        }
        nextJumpVector = normal_sum;
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
            }

            Vector2 vector_move = new Vector2(x_move, 0.0F);

            rb.AddForce(vector_move * moveSpeed);
            // Up rotates clockwise, down rotates counterclockwise
            rb.AddTorque(-r_move * rotationSpeed);
        }
    }

    // Returns the color object of the player.
    public Color getColor()
    {
        return GetComponent<SpriteRenderer>().color;
    }
}