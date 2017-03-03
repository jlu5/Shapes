using UnityEngine;

public class Door : Collidable {
    public int ID;
    public int targetDoor;
    public bool isLocked;
    private GameObject doorLock;
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
    }
    
    // PlayerHit is a no-op
	public override void PlayerHit (Player player) {
	}

    public override void PlayerInteract(Player player)
    {
        Collidable otherDoor = GameState.Instance.GetCollidable<Door>(targetDoor);
        if (otherDoor == null || isLocked)
        {
            // XXX make this obvious to the player outside the editor
            if (otherDoor == null)
            {
                Debug.LogWarning(string.Format("This door is locked! (points to invalid door {0})", targetDoor));
            } else
            {
                Debug.Log("This door is locked!");
            }
        }
        else
        {
            // Teleport the player!
            player.gameObject.transform.position = otherDoor.gameObject.transform.position;
        }
    }


}
