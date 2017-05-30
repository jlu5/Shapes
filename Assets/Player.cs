/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Player.cs: Implements the base player character
 */
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {
    // Color presets
    public static Color textLabelColor = new Color(0, 0, 0, 0.5F);
    public static string[] playerColors = new string[] { "#7CC3FF", "#FFA574", "#74FF92", "#C274FF", "#FCFF74", "#FF798D"};

    // Max amount of collisions to fetch at a given time.
    public const int MaxCollisionCount = 64;

    // Public (editor-exposed) variables
    public float moveSpeed = 10;
    public float jumpStrength = 5.0f; // Jump strength force relative to player mass 
    public float rotationSpeed = 3;
    public float jumpRecoilStrength = 1.0f;
    public int playerID = 0;

    // Quick access to components & resources
    public Rigidbody2D rb; // "rb" for RigidBody
    public ParticleSystem ps;
    private GameObject bindDisplayTemplate; // BindDisplayObject template
    private GameObject simpleTextMesh;
    private GameObject playerIDLabel;
    public GameObject feet;
    public GameObject spheresContainer;

    // Jump / Rigidbody basics tracking
    private bool canJump = false;

    // These lists are going away...
    public List<GameObject> activeTriggers;

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
        // Find our Rigidbody2D and ParticleSystem components
        rb = GetComponent<Rigidbody2D>();
        ps = GetComponentInChildren<ParticleSystem>();

        bindDisplayTemplate = Resources.Load<GameObject>("BindDisplayObject");

        if (playerID == 0) // Player ID should never be zero (this is the prefab default)
        {
            Debug.LogWarning("Player object has invalid ID 0!");
        }
        else if (playerID <= playerColors.Count())
        {
            // Set the player color to the preset color for the current player ID, if one exists.
            string hexcolor = playerColors[playerID-1];
            Debug.Log(string.Format("Setting color of player {0} to {1}", playerID, hexcolor));
            GetComponent<SpriteRenderer>().color = Utils.HexColor(hexcolor);
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

    void Start()
    {
        // Add this player to our global game state.
        GameState.Instance.AddPlayer(playerID, this);
        GameState.Instance.playerCount++;

        // Set the particle system to match the player color.
        ParticleSystem.MainModule psmain = ps.main;
        Color myColor = getColor();
        // Use a gradient that slowly fades from the player color to emptiness.
        psmain.startColor = new ParticleSystem.MinMaxGradient(myColor, new Color(myColor.r, myColor.g, myColor.b, 0));

        // The "feet" graphic is used to show the superjump powerup.
        feet = transform.Find("PlayerFeet").gameObject;
        // Make it also match the player color.
        feet.GetComponent<SpriteRenderer>().color = myColor;
        feet.SetActive(false);

        // The "spheres" graphic is used to show the speed powerup - its code is in PowerupSpeed.cs
        spheresContainer = transform.Find("PlayerSpinningSpheres").gameObject;
    }

    /*
    void OnCollisionStay2D(Collision2D col)
    {
        /* Make jump realistic: whenever the player collides with the environment, set the
         * jump direction perpendicular to the surface(s) the player is touching.
         * This is in contrast to making jump always point upwards, which is incorrect
         * when colliding with slanted platforms or the bottom of floating objects.
         * Since directions are expressed as a vector, we can sum all the collision normal
         * vectors if the player is touching multiple points at once (i.e. corner jump is possible).        
        Vector2 normalSum = Vector2.zero;
        ContactPoint2D[] collisions = new ContactPoint2D[MaxCollisionCount];
        rb.GetContacts(collisions);

        foreach (ContactPoint2D contactPoint in collisions)
        {
            if (contactPoint.otherRigidbody == null)
            {
                // World items should always have a rigid body attached for jump processing to work.
                // If this is missing, warn the user.
                Debug.LogWarning(string.Format("Player: Skipping processing collision with object with no rigidbody! (Player ID: {0})",
                                               playerID));
                continue;
            }

            // Do some math to make heavier objects count for more when calculating jump forces.
            normalSum += (contactPoint.normal * contactPoint.otherRigidbody.mass);
        }
        
        // Normalize the resulting vector so we don't get super jumps when colliding with multiple
        // objects at the same angle (e.g. when resting on the crack between two parallel platforms).
        normalSum.Normalize();
        nextJumpVector = normalSum;
    }*/

    // Attempts to attach to all colliding players.
    public void Attach()
    {
        Collider2D[] collisions = new Collider2D[MaxCollisionCount];
        rb.GetContacts(collisions);

        foreach (Collider2D collider in collisions)
        {
            GameObject playerObject = collider.gameObject;

            // Create a relative (angle and distance preserving) joint between the two objects.
            RelativeJoint2D joint = gameObject.AddComponent<RelativeJoint2D>();
            joint.connectedBody = playerObject.GetComponent<Rigidbody2D>();
            playerBinds[playerObject] = joint;

            // Don't track collisions between the player and other bounded ones;
            joint.enableCollision = false;

            // Track which player has the RelativeJoint2D, so that other players
            // can unbind themselves.
            Player otherPlayer = playerObject.GetComponent<Player>();
            if (otherPlayer.playerID == playerID)
            {
                continue;
            }
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

    void OnCollisionEnter2D(Collision2D col)
    {
        // Objects with specific collision routines are (e.g. finishes) called here.
        // These each have a script component deriving from the Collidable class,
        // and override PlayerHit() to make this work.
        Collidable collidable = col.gameObject.GetComponent<Collidable>();
        if (collidable != null)
        {
            collidable.PlayerHit(this);
            return; // Stop processing here.
        }

        /*
        // Collision with other players are tracked separately; this is used to process player
        // attachment.
        if (col.gameObject.CompareTag("Player")) {
            collidingPlayers.Add(col.gameObject);
        }
        */
        
        canJump = true;
    }


    // Update is called once per frame
    void Update() {
        if (GameState.Instance.currentPlayer == playerID) {
            // Get horizontal and vertical (rotation) movement
            float x_move = Input.GetAxis("Horizontal");
            float r_move = Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Jump") && canJump)
            {
                Vector2 jumpVector = Vector2.zero;

                Debug.Log("Player " + playerID + " jumped");
                ContactPoint2D[] collisions = new ContactPoint2D[MaxCollisionCount];
                rb.GetContacts(collisions);

                foreach (ContactPoint2D contactPoint in collisions)
                {
                    if (contactPoint.otherRigidbody == null)
                    {
                        // World items should always have a rigid body attached for jump processing to work.
                        // If this is missing, warn the user.
                        Debug.LogWarning(string.Format("Player: Skipping processing collision with object with no rigidbody! (Player ID: {0})",
                                                       playerID));
                        continue;
                    }

                    // Do some math to make heavier objects count for more when calculating jump forces.
                    jumpVector += (contactPoint.normal * contactPoint.otherRigidbody.mass);
                }

                // Normalize the resulting vector so we don't get super jumps when colliding with multiple
                // objects at the same angle (e.g. when resting on the crack between two parallel platforms).
                jumpVector.Normalize();

                Debug.Log("Jump vector: " + jumpVector.ToString());

                // Add a force on the player character to propel it perpendicular to the
                // surface(s) it's touching.
                Vector2 jumpForce = jumpVector * jumpStrength;
                Debug.Log(string.Format("Player {0} jumps with a force of {1}", playerID, jumpForce));
                rb.AddForce(jumpForce, ForceMode2D.Impulse);

                foreach (ContactPoint2D contactPoint in collisions)
                {
                    // For each object that we're colliding with, exert an opposing force
                    // when we jump (if that object has a rigid body).
                    if (contactPoint.otherRigidbody)
                    {
                        contactPoint.otherRigidbody.AddForce(-jumpVector * jumpRecoilStrength * rb.mass);
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
            rb.AddForce(vector_move * moveSpeed * rb.mass);
            // Up rotates clockwise, down rotates counterclockwise.
            rb.AddTorque(r_move * rotationSpeed);
        }

        // Force the player ID label (and other subobjects) to be always upright
        foreach (Transform child in transform) {
            child.transform.rotation = Quaternion.identity;
        }

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
