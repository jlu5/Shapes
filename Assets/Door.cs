/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Door.cs: Lockable doors for Shapes, teleporting the player to its target on interact
 */
using UnityEngine;

public class Door : Collidable {
    public int ID;
    public int targetDoor;
    public bool isLocked;
    private GameObject doorLock;
    private GameObject bindDisplay;
    private GameObject bindDisplayTemplate;
    public Color color;
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

        color = GetComponent<SpriteRenderer>().color;
        if (isLocked)
        {
            // If the door is locked, show a lock overlay.
            doorLock = Instantiate(Resources.Load<GameObject>("DoorLockDisplay"));
            doorLock.transform.SetParent(transform, false);
            // Offset the colour of the door lock so that it doesn't blend in with the door.
            doorLock.GetComponent<SpriteRenderer>().color = color + lockOffsetColor;
        }

        bindDisplayTemplate = Resources.Load<GameObject>("BindDisplayObject");

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

        // Create a bind display between the doors if one doesn't already exist.
        Collidable otherDoor = GameState.Instance.GetCollidable<Door>(targetDoor);
        if (bindDisplay == null && otherDoor && other.gameObject.CompareTag("Player"))
        {
            bindDisplay = Instantiate(bindDisplayTemplate);
            BindDisplay bindDisplayScript = bindDisplay.GetComponent<BindDisplay>();

            bindDisplayScript.object1 = gameObject;
            bindDisplayScript.object2 = otherDoor.gameObject;
            bindDisplayScript.transform.SetParent(transform);
        }
	}

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (bindDisplay != null) {
            Destroy(bindDisplay);
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
