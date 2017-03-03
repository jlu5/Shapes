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

    public void Unlock() {
        isLocked = false;
        if (doorLock)
        {
            // Remove the door lock overlay if it exists.
            Destroy(doorLock);
        }
    }

    // PlayerHit is a no-op
	public override void PlayerHit(Player player) {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Call the base Collidable class' trigger code.
        base.OnTriggerEnter2D(other);

        // Create a bind display between the doors if one doesn't already exist.
        Collidable otherDoor = GameState.Instance.GetCollidable<Door>(targetDoor);
        if (bindDisplay == null && otherDoor)
        {
            bindDisplay = Instantiate(bindDisplayTemplate);
            BindDisplay bindDisplayScript = bindDisplay.GetComponent<BindDisplay>();

            bindDisplayScript.object1 = gameObject;
            bindDisplayScript.object2 = otherDoor.gameObject;
            bindDisplayScript.transform.SetParent(transform);
        }
	}

    void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (bindDisplay != null) {
            Destroy(bindDisplay);
        }
    }

    public override void PlayerInteract(Player player)
    {
        Collidable otherDoor = GameState.Instance.GetCollidable<Door>(targetDoor);
        if (otherDoor == null || isLocked)
        {
            // XXX make this obvious to the player outside the editor
            Debug.Log("This door is locked!");
        }
        else
        {
            // Teleport the player!
            player.gameObject.transform.position = otherDoor.gameObject.transform.position;
        }
    }


}
