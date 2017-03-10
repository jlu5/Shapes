/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Player.cs: Implements the base player character
 */
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {
    // Public (editor-exposed) variables
    public float moveSpeed = 10;
    public float jumpStrength = 700;
    public float rotationSpeed = 3;
    public float jumpRecoilStrength = 100;
    public Color textLabelColor = new Color(0, 0, 0, 0.5F);
    public int playerID = 0;

    // Quick access to components & resources
    private Rigidbody2D rb; // "rb" for RigidBody
    private GameObject bindDisplayTemplate; // BindDisplayObject template
    private GameObject simpleTextMesh;
    private GameObject playerIDLabel;

    // Jump / Rigidbody basics tracking
    private bool canJump = false;
    private Vector2 nextJumpVector;
    private Dictionary<GameObject, Vector2> collidingObjects = new Dictionary<GameObject, Vector2>();
    public List<GameObject> activeTriggers;

    // Tracking for player attachments
    private List<GameObject> collidingPlayers = new List<GameObject>();

    // Track player objects and the joints binding them to one another.
    protected Dictionary<GameObject, RelativeJoint2D> playerBinds = new Dictionary<GameObject, RelativeJoint2D>();
    // Track an index of bind display lines.
    protected Dictionary<int, GameObject> bindDisplays = new Dictionary<int, GameObject>();

    // Tracks which player GameObject is leading the current player attachment (i.e. which player
    // has the RelativeJoint2D component).
    protected List<GameObject> masterPlayers = new List<GameObject>();
    protected int bindCount = 0;

    // Initialization is done in two steps here: Awake, which runs first, and Start, which runs second.
    // This is so that variables other scripts depend on are always created (step 1) before any
    // cross-script communication is done (step 2).
    void Awake () {
        // Find our Rigidbody2D object
        rb = GetComponent<Rigidbody2D>();
        bindDisplayTemplate = Resources.Load<GameObject>("BindDisplayObject");

        if (playerID == 0) // Player ID should never be zero (this is the prefab default)
        {
            Debug.LogWarning("Immovable player object with invalid ID!");
        }

        simpleTextMesh = Resources.Load<GameObject>("SimpleTextMesh");

        // Create a small text label to denote which player is which
        playerIDLabel = Instantiate(simpleTextMesh);
        playerIDLabel.transform.SetParent(gameObject.transform, false);
        TextMesh playerIDText = playerIDLabel.GetComponent<TextMesh>();
        playerIDText.text = playerID.ToString();
        // Use a smaller font size
        playerIDText.fontSize /= 3;
        playerIDText.color = textLabelColor;

        // Sort the label so that it shows over the player sprite.
        Renderer renderer = playerIDLabel.GetComponent<Renderer>();
        renderer.sortingLayerName = "PlayerForeground";
        renderer.sortingOrder = -1;
    }

    void Start () {
        // Add this player to our global game state.
        GameState.Instance.AddPlayer(playerID, this);
        GameState.Instance.playerCount++;
    }

    void UpdateCollisionAngles(Collision2D col)
    {
        // Track the objects we're currently colliding with along with the collision
        // angle. This is used to process jump.
        Vector2 normal = new Vector2();
        // XXX: ugly but IEnumerable.Sum() only allows summing numbers AFAIK.
        foreach (ContactPoint2D contact in col.contacts)
        {
            normal += contact.normal;
        }
        collidingObjects[col.gameObject] = normal;
    }

    // Collision handlers
    void OnCollisionEnter2D(Collision2D col)
    {
        // Objects with specific collision routines are (e.g. finishes) called here.
        // These each have a script component deriving from the Collidable class,
        // and override PlayerHit() to make this work.
        Collidable collidable = col.gameObject.GetComponent<Collidable>();
        if (collidable != null)
        {
            collidable.PlayerHit(this);
        }

        // Collision with other players are tracked separately; this is used to process player
        // attachment.
        if (col.gameObject.CompareTag("Player")) {
            collidingPlayers.Add(col.gameObject);
        }

        // Track which objects we're colliding with.
        UpdateCollisionAngles(col);
        canJump = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        // Remove the other object from the list of objects we're colliding with.
        collidingObjects.Remove(col.gameObject);

        if (collidingObjects.Count == 0)
        {
            // Disable jump when leaving all collisions, so that we can't magically gain momentum in mid air.
            canJump = false;
        }

        if (col.gameObject.CompareTag("Player"))
        {
            collidingPlayers.Remove(col.gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        UpdateCollisionAngles(col);
        /* Make jump realistic: whenever the player collides with the environment, set the
         * jump direction perpendicular to the surface(s) the player is touching.
         * This is in contrast to making jump always point upwards, which is incorrect
         * when colliding with slanted platforms or the bottom of floating objects.
         * Since directions are expressed as a vector, we can sum all the collision normal
         * vectors if the player is touching multiple points at once (i.e. corner jump is possible).
         */
        Vector2 normal_sum = Vector2.zero;

        // Sum up the vectors at which we're colliding with all other objects.
        foreach (Vector2 vector in collidingObjects.Values)
        {
            normal_sum += vector;
        }
        nextJumpVector = normal_sum;

        // Normalize the resulting vector so we don't get super jumps when colliding with multiple
        // objects at the same angle (e.g. when resting on the crack between two parallel platforms).
        nextJumpVector.Normalize();
    }

    // Attempts to attach to all colliding players.
    public void Attach()
    {
        foreach (GameObject playerObject in collidingPlayers)
        {
            // Create a relative (angle and distance preserving) joint between the two objects.
            RelativeJoint2D joint = gameObject.AddComponent<RelativeJoint2D>();
            joint.connectedBody = playerObject.GetComponent<Rigidbody2D>();
            playerBinds[playerObject] = joint;

            // Don't track collisions between the player and other bounded ones;
            joint.enableCollision = false;

            // Track which player has the RelativeJoint2D, so that other players
            // can unbind themselves.
            Player otherPlayer = playerObject.GetComponent<Player>();
            otherPlayer.masterPlayers.Add(gameObject);

            // Make sure we're keeping track of the number of players bounded.
            bindCount++;
            otherPlayer.bindCount++;

            // Create a new bind display object from our prefab, if one
            // doesn't exist already.
            if (!bindDisplays.ContainsKey(otherPlayer.playerID))
            {
                GameObject bdo = Instantiate(bindDisplayTemplate);
                // Show what player IDs this bind display object uses
                bdo.name += string.Format("{0}-{1}", playerID, otherPlayer.playerID);
                bdo.transform.SetParent(transform);

                // Set the bind display script to track this character and the other character.
                BindDisplay bdoScript = bdo.GetComponent<BindDisplay>();
                bdoScript.object1 = gameObject;
                bdoScript.object2 = playerObject;

                bindDisplays[otherPlayer.playerID] = bdo;
            }
        }
    }

    // Detaches from currently attached players.
    public void Detach()
    {
        foreach (GameObject masterObject in masterPlayers)
        {
            // If we're not the master player in an attachment, the relative
            // joint we need to delete will be in another player object.
            Player masterPlayer = masterObject.GetComponent<Player>();
            Dictionary<GameObject, RelativeJoint2D> masterPlayerBinds = masterPlayer.playerBinds;
            try
            {
                RelativeJoint2D joint = masterPlayerBinds[gameObject];
                Destroy(joint);
            }
            catch (KeyNotFoundException)
            {
                // The joint went missing, ignore
            }
            masterPlayerBinds.Remove(gameObject);
            masterPlayer.bindCount--;

            // Remove any bind displays for us if they exist.
            if (masterPlayer.bindDisplays.ContainsKey(playerID))
            {
                Destroy(masterPlayer.bindDisplays[playerID]);
                masterPlayer.bindDisplays.Remove(playerID);
            }
        }
        masterPlayers.Clear();

        foreach (RelativeJoint2D joint in GetComponents<RelativeJoint2D>())
        {
            // Remove any old player binds belonging to this character.
            GameObject otherObject = joint.connectedBody.gameObject;
            playerBinds.Remove(otherObject);
            Destroy(joint);
            otherObject.GetComponent<Player>().bindCount--;
        }

        foreach (KeyValuePair<int, GameObject> kvp in bindDisplays)
        {
            // Destroy all bind displays under this object.
            Destroy(kvp.Value);
        }
        bindDisplays.Clear();
    }

    // Interact with all triggered Collidables (for collidables that have trigger-based rigid bodies
    // instead of physical collisions)
    public void InteractAll()
    {
        foreach (GameObject otherObject in activeTriggers)
        {
            Collidable collidable = otherObject.GetComponent<Collidable>();
            if (collidable != null)
            {
                collidable.PlayerInteract(this);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (GameState.Instance.currentPlayer == playerID) {
            // Get horizontal and vertical (rotation) movement
            float x_move = Input.GetAxis("Horizontal");
            float r_move = Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Jump") && canJump)
            {
                // Add a force on the player character to propel it perpendicular to the
                // surface(s) it's touching.
                rb.AddForce(nextJumpVector * jumpStrength);

                foreach (KeyValuePair<GameObject, Vector2> objpair in collidingObjects.ToList())
                {
                    if (objpair.Key != null)
                    {
                        // For each object that we're colliding with, exert an opposing force
                        // when we jump (if that object has a rigid body).
                        Rigidbody2D other_rb = objpair.Key.GetComponent<Rigidbody2D>();
                        if (other_rb != null)
                        {
                            other_rb.AddForce(-nextJumpVector * jumpRecoilStrength);
                        }
                    } else
                    {
                        // The other object was destroyed while we collided with it; remove it.
                        collidingObjects.Remove(objpair.Key);
                    }
                }

                // Disable jump while we're in mid-air (this is reset in OnCollisionEnter2D).
                canJump = false;
                Debug.Log("canJump set to false for player ID " + playerID);
            }
            else if (Input.GetButtonDown("Attach"))
            {
                Attach();
            }
            else if (Input.GetButtonDown("Detach"))
            {
                Detach();
            }
            else if (Input.GetButtonDown("Submit"))
            {
                InteractAll();
            }

            Vector2 vector_move = new Vector2(x_move, 0.0F);
            rb.AddForce(vector_move * moveSpeed);
            // Up rotates clockwise, down rotates counterclockwise. TODO: replace this with inversion in the input settings
            rb.AddTorque(-r_move * rotationSpeed);
        }
    }

    void LateUpdate() {
        // Force the player ID label to be always upright
        playerIDLabel.transform.rotation = Quaternion.identity;

        if (GameState.Instance.currentPlayer == playerID)
        {
            // Handle camera pans
            {
                // Don't mess with the camera's Z axis, or the screen will blank out...
                Vector3 target = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
                Vector3 velocity = Vector3.zero;

                // Use Unity's built in damping to make the panning less choppy
                Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position,
                    target, ref velocity, GameState.Instance.cameraPanTime);
            }
        }
    }

    // Handles mouse clicks on the player, which sets it to the current one.
    void OnMouseUp()
    {
        GameState.Instance.currentPlayer = playerID;
    }

    // Returns the color object of the player. TODO: just make color a public attribute...
    public Color getColor()
    {
        return GetComponent<SpriteRenderer>().color;
    }
}
