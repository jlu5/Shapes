/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Door.cs: Lockable doors for Shapes, teleporting the player to its target on interact.
 */
using UnityEngine;
using System;

public class Door : Collidable {
    [Tooltip("Sets the internal door ID.")]
    public int ID;
    [Tooltip("Sets the target door ID.")]
    public int targetDoor;
    [Tooltip("Sets the whether the door is locked.")]
    public bool isLocked;
    [Tooltip("Sets the whether a target door hint should be shown.")]
    public bool useHints = true;

    // Color of the door
    public Color color {get; set;}

    protected bool isHinted;  // Defines whether we're showing the target door hint right now
    private GameObject doorLock;  // Points to the attached door lock sprite
    private SpriteRenderer spriteRenderer;

    // Offset color used to display hints when the player hits the door.
    public static Color hintOffsetColor = new Color(0.1F, 0.1F, 0.1F, 0);
    // Offset color used for displaying locks (so they don't blend in completely with the door)
    public static Color lockOffsetColor = new Color(0.2F, 0.2F, 0.2F, -0.2F);

    void Start()
    {
        // Initialization sanity checks...
        if (targetDoor == ID)
        {
            Debug.LogError(string.Format("Door {0} has target of itself; removing!", ID));
            Destroy(gameObject);
        }

        GameState.Instance.RegisterGameScript(ID, this);
	spriteRenderer = GetComponent<SpriteRenderer>();
	color = spriteRenderer.color;

        if (isLocked)
        {
            // If the door is locked, show a lock overlay.
            doorLock = Instantiate(Resources.Load<GameObject>("DoorLockDisplay"));
            doorLock.transform.SetParent(transform, false);
            // Offset the colour of the door lock so that it doesn't blend in with the door.
            doorLock.GetComponent<SpriteRenderer>().color = Utils.ColorSum(color, lockOffsetColor);
        }

    }

    public void UpdateColor(Color newColor) {
        spriteRenderer.color = newColor;
    }

    // Method to unlock the door.
    public void Unlock() {
        isLocked = false;
        if (doorLock)
        {
            // Remove the locked door overlay if it exists.
            Destroy(doorLock);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // Call the base Collidable class' trigger code.
        base.OnTriggerEnter2D(other);

        // Only process this trigger if it's a player that we're colliding with
        if (other.gameObject.GetComponent<Player>() == null)
        {
            return;
        }

        // Hint at where the door goes by tinting the color of the source and color doors. This is
        // subtle enough to not give away rough locations if the target door is off the screen
        // anyways.
        Door otherDoor = (Door) GameState.Instance.GetGameScript<Door>(targetDoor);
        if (otherDoor != null && useHints && !isHinted)
        {
	    isHinted = true;
            // For both the source and target doors, add the current door color and the
            // hint offset color.
            Color mycolor = Utils.ColorSum(color, hintOffsetColor);
            UpdateColor(mycolor);

            Color theircolor = Utils.ColorSum(otherDoor.color, Door.hintOffsetColor);
            otherDoor.UpdateColor(theircolor);
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);

        if (other.gameObject.GetComponent<Player>() == null)
        {
            return;
        }

        Door otherDoor = (Door)GameState.Instance.GetGameScript<Door>(targetDoor);
        if (isHinted)
        {
            // Unset hinting door coloring if it is set.
            if (otherDoor != null)
            {
                otherDoor.isHinted = false;
                // Door.color is the original color, so we can simply reset it back to that.
                otherDoor.UpdateColor(otherDoor.color);
            }
            isHinted = false;
            UpdateColor(color);
        }
    }

    // Method called when the player interacts with this door.
    public override void PlayerInteract(Player player)
    {
        // Find the other door collidable (registered in GameState).
        Door otherDoor = (Door) GameState.Instance.GetGameScript<Door>(targetDoor);

        if (otherDoor == null)
        {
            // If the target door doesn't exist, treat the door as locked.
            otherDoor = this;
            Debug.LogWarning(string.Format("Invalid target door {0}; setting target to current door!", targetDoor));
        }

        if (isLocked)
        {
            // The door is explicitly locked: warp the player to the door they started in.
            otherDoor = this;
            Debug.Log("This door is locked!");
        }

        // Unbind all players. XXX: maybe teleport attached players instead?
        player.Detach();

        // Teleport the player to the target door!
        player.gameObject.transform.position = otherDoor.gameObject.transform.position;
    }


}
