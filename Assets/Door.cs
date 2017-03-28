/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Door.cs: Lockable doors for Shapes, teleporting the player to its target on interact
 */
using UnityEngine;

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
	public Color hintOffsetColor = new Color(0.1F, 0.1F, 0.1F, 0);
    // Offset color used for displaying locks (so they don't blend in completely with the door)
    // and hinting the target door
    public Color lockOffsetColor = new Color(0.6F, 0.6F, 0.6F, 0);

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
			doorLock.GetComponent<SpriteRenderer>().color = color + lockOffsetColor;
        }

    }

	public void UpdateColor() {
		spriteRenderer.color = color;
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
            otherDoor.color += hintOffsetColor;
			color += hintOffsetColor;
			isHinted = true;

			UpdateColor();
			otherDoor.UpdateColor();
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
			if (otherDoor != null)
			{
				otherDoor.color -= hintOffsetColor;
				otherDoor.isHinted = false;
				otherDoor.UpdateColor();
			}
			color -= hintOffsetColor;
			isHinted = false;
			UpdateColor();
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
