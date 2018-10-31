/* Shapes Game (c) 2017-2018 James Lu. All rights reserved.
 * Player.cs: Implements the base of the player character.
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
    [Tooltip("Sets the speed of the player's movement.")]
    public float moveSpeed = 0.5f;
    [Tooltip("Sets the strength of the player's jump, relative to player mass.")]
    public float jumpStrength = 5;
    [Tooltip("Sets the strength of the player's jump recoil; i.e. what force should be applied on the objects the player is jumping off of.")]
    public float jumpRecoilStrength = 1;
    [Tooltip("Sets the rotation strength of the player (torque).")]
    public float rotationSpeed = 3;
    [Tooltip("Sets the the player ID. This should NOT be set to zero or duplicated (i.e. each player must have a unique ID!).")]
    public int playerID = 0;
    [Tooltip("Sets whether the player should swim or fly in the cardinal up/down directions if this is set to false, the player flies in the direction it is facing based on its current rotation.")]
    public bool flyInCardinalDirections = false;

    // Quick access to components & resources
    public Rigidbody2D rb {get; protected set;} // "rb" for RigidBody
    private GameObject simpleTextMesh;
    private GameObject playerIDLabel;
    public Color color { get; protected set; }
    public Sprite sprite {get; protected set; }

    private BindEngine bindEngine;
    public PlayerGFX gfx {get; protected set; }

    /* Track whether jumping and flying / swimming (these share the same code) are allowed.
     * Jump is disabled in mid-air, and enabled when the player hits the ground or any solid object.
     * Flying is a number that essentially counts the number of "reasons" why a player can fly at any given time;
     * if the value is > 0, it is considered enabled (water adds 1 when the player is underwater while rockets add 1
     * as long as the powerup is active).
     */
    public bool canJump {get; set;}
    public int canFly {get; set;}
    // Sets the flying/swimming speed: Water and rockets add to this so speed for e.g. having a rocket underwater stacks
    public float flySpeed { get; set; }

    // Track whether this player has entered a finish prior to being removed.
    public bool finished {get; set;}

    // Track the triggers we're interacting with. This list is updated by the Collidable class.
    public List<GameObject> activeTriggers {get; set;}

    // Track whether the player is visible on camera (used for quick switching between players off screen)
    public bool visible { get; protected set; }
    public bool triedPanning { get; set; }

    // Initialization is done in two steps here: Awake, which runs first, and Start, which runs second.
    // This is so that variables other scripts depend on are always created (step 1) before any
    // cross-script communication is done (step 2).
    void Awake () {
        // Find our Rigidbody2D and ParticleSystem components
        rb = GetComponent<Rigidbody2D>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (playerID == 0) // Player ID should never be zero (this is the prefab default)
        {
            Debug.LogWarning("Player object has invalid ID 0!");
        }
        else if (playerID <= playerColors.Count())
        {
            // Set the player color to the preset color for the current player ID, if one exists.
            string hexcolor = playerColors[playerID-1];
            Debug.Log(string.Format("Setting color of player {0} to {1}", playerID, hexcolor));
            spriteRenderer.color = Utils.HexColor(hexcolor);
        }
        color = spriteRenderer.color;  // Set our color attribute for other scripts
        sprite = spriteRenderer.sprite;  // Ditto for our current sprite

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

        bindEngine = gameObject.AddComponent<BindEngine>();
        gfx = gameObject.AddComponent<PlayerGFX>();
        gfx.color = color;
    }

    void OnDestroy() {
        if (GameState.Instance && GameState.Instance.playerDeathFatal && !GameState.Instance.gameEnded && !finished) {
            // The player never reached a finish before being destroyed, so end the game.
            GameState.Instance.LevelEnd(false);
            GameState.Instance.players.Remove(playerID);
        }
    }

    void Start()
    {
        if (GameState.Instance)
        {
            // Add this player to our global game state.
            GameState.Instance.players[playerID] = this;
            GameState.Instance.playerList.AddPlayer(playerID, this);
            GameState.Instance.playerCount++;
        } else {
            Debug.LogError("Player object added with no GameState in scene! Good luck moving *anything*");
        }

        activeTriggers = new List<GameObject>();
    }


    void Jump(bool skipJumpCheck=false) {
        if (!canJump && !skipJumpCheck)
        {
            // We are in mid air, so don't allow the jump to happen!
            return;
        }
        Vector2 jumpVector = Vector2.zero;

        Debug.Log("Player " + playerID + " jumped");
        ContactPoint2D[] collisions = new ContactPoint2D[MaxCollisionCount];
        rb.GetContacts(collisions);

        foreach (ContactPoint2D contactPoint in collisions)
        {
            if (contactPoint.otherRigidbody == null)
            {
                // Ignore items that don't have a rigid body.
                continue;
            }

            // Do some math to make heavier objects count for more when calculating jump forces.
            jumpVector += (contactPoint.normal * contactPoint.otherRigidbody.mass);
        }

        // Normalize the resulting vector so we don't get super jumps when colliding with multiple
        // objects at the same angle (e.g. when resting on the crack between two parallel platforms).
        jumpVector.Normalize();

        Debug.Log("Jump vector: " + jumpVector.ToString());

        // Jump perpendicular to the surface(s) the player is touching.
        Vector2 jumpForce = jumpVector * jumpStrength * Mathf.Abs(rb.gravityScale) * rb.mass;
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

    void Fly() {
        float movement = Input.GetAxis("Fly");

        /* Calculate a target force based off:
         * 1) The object's current rotation, if cardinal directions are disabled.
         * 2) The fly/swim speed
         * 3) The current conditions of gravity
         * 4) The player's mass
         */
        // Note: we must put the base force first because "Quaternion (rotation) * Vector2/3" is defined, but the opposite is not.
        Vector3 baseForce = flyInCardinalDirections ? Vector3.up : transform.localRotation * Vector3.up;
        Vector2 force = (Vector2) baseForce * flySpeed * -Physics2D.gravity.y * Mathf.Abs(Mathf.Max(rb.gravityScale, 1)) * rb.mass;

        Debug.Log("Player: flying by " + (movement * force).ToString());
        rb.AddForce(force*movement, ForceMode2D.Impulse);

        gfx.SetRocketEmission(movement);
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
        canJump = true;
    }

    public void Attach() {
        bindEngine.Attach();
    }

    public void Detach() {
        bindEngine.Detach();
    }

    // Update runs every frame; this works better for handling keys as they are pressed
    void Update()
    {
        if (GameState.Instance && GameState.Instance.currentPlayer == playerID)
        {
            // Handle jump, attach, detach, interact:
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
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
        }
    }

    // Fixed update is frame rate independent and used to track movement and camera updates
    void FixedUpdate() {
        if (GameState.Instance && GameState.Instance.currentPlayer == playerID) {
            // Get horizontal and vertical (rotation) movement
            float x_move = Input.GetAxis("Horizontal");
            float r_move = Input.GetAxis("Vertical");

            Vector2 vector_move = new Vector2(x_move, 0.0F);
            rb.AddForce(vector_move * moveSpeed * rb.mass, ForceMode2D.Impulse);
            // Up rotates clockwise, down rotates counterclockwise.
            rb.AddTorque(r_move * rotationSpeed * rb.mass);

            if (canFly >= 1)
            {
                Fly();
            }

            // Handle camera pans:
            // Don't mess with the camera's Z axis, or the screen will blank out...
            Vector3 target = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            Vector3 velocity = Vector3.zero;

            // Use Unity's built in damping to make the panning less choppy
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position,
                target, ref velocity, GameState.Instance.cameraPanTime);
        }

        // Force the player ID label (and other subobjects) to be always upright
        foreach (Transform child in transform) {
            child.transform.rotation = Quaternion.identity;
        }
    }

    // Handles mouse clicks on the player, which sets it to the current one.
    void OnMouseUp()
    {
        if (GameState.Instance)
        {
            GameState.Instance.currentPlayer = playerID;
        }
    }

    void OnBecameVisible()
    {
        visible = true;
        triedPanning = false;
    }

    void OnBecameInvisible()
    {
        visible = false;
    }
}
