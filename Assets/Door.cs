/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Door.cs: Lockable doors for Shapes, teleporting the player to its target on interact
 */
using UnityEngine;
using System;

public class Door : Collidable {
    public int ID;  // Internal door ID
    public int targetDoor;
    public bool isLocked;
    public bool useHints = true;  // Defines whether we should use the target door hint.
    public Color color;  // Color of the door

	protected bool isHinted;  // Defines whether we're showing the target door hint right now
	private GameObject doorLock;  // Points to the attached door lock sprite
	private SpriteRenderer spriteRenderer;

    // Offset color used to display hints when the player hits the door.
    protected Color hintOffsetColor = new Color(0.1F, 0.1F, 0.1F, 0);
    // Offset color used for displaying locks (so they don't blend in completely with the door)
    protected Color lockOffsetColor = new Color(0.2F, 0.2F, 0.2F, -0.2F);

    // Returns a normalized sum of the two colors.
    public Color ColorSum(Color first, Color second)
    {
        // Separate the components, add them, and cap them at 1.
        Debug.Log(string.Format("first color values: {0}, {1}, {2}, {3}", first.r, first.g, first.b, first.a));
        Debug.Log(string.Format("second color values: {0}, {1}, {2}, {3}", second.r, second.g, second.b, second.a));
        float r = Math.Min(1, first.r + second.r);
        float g = Math.Min(1, first.g + second.g);
        float b = Math.Min(1, first.b + second.b);
        float a = Math.Min(1, first.a + second.a);
        // Return the new color sum.
        Debug.Log(string.Format("sum: {0}, {1}, {2}, {3}", r, g, b, a));
        return new Color(r, g, b, a);
    }

    void Start()
    {
        // Initialization sanity checks...
        if (targetDoor == ID)
        {
            Debug.LogError(string.Format("Door {0} has target of itself; removing!", ID));
            Destroy(gameObject);
        }
        GameState.Instance.RegisterCollidable(ID, this);

		spriteRenderer = GetComponent<SpriteRenderer>();
		color = spriteRenderer.color;
        if (isLocked)
        {
            // If the door is locked, show a lock overlay.
            doorLock = Instantiate(Resources.Load<GameObject>("DoorLockDisplay"));
            doorLock.transform.SetParent(transform, false);
            // Offset the colour of the door lock so that it doesn't blend in with the door.
            doorLock.GetComponent<SpriteRenderer>().color = ColorSum(color, lockOffsetColor);
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
        Door otherDoor = (Door) GameState.Instance.GetCollidable<Door>(targetDoor);
        if (otherDoor != null && useHints && !isHinted)
        {
			isHinted = true;
            // For both the source and target doors, add the current door color and the
            // hint offset color.
            Color mycolor = ColorSum(color, hintOffsetColor);
            UpdateColor(mycolor);

            Color theircolor = ColorSum(otherDoor.color, otherDoor.hintOffsetColor);
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

        Door otherDoor = (Door)GameState.Instance.GetCollidable<Door>(targetDoor);
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
        Collidable otherDoor = GameState.Instance.GetCollidable<Door>(targetDoor);

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
