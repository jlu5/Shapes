using UnityEngine;

public class DoorKey : Collidable {
    public int doorID;
    private Door myDoor;

    // Note: this start method requires the "Door" script to be executed before it! (Unity Script Execution Order)
    void Start() {
        // Explicitly cast this from Collidable to Door.
        // XXX: look into making GetCollidable explicitly return items with their specific types.
        myDoor = (Door) GameState.Instance.GetCollidable<Door>(doorID);

        // Sanity checks to make sure the existance of this key makes sense.
        if (myDoor == null)
        {
            Debug.LogWarning(string.Format("DoorKey with invalid target door {0}, removing! If this is incorrect, make sure that this script is set to execute after the Door script!", doorID));
            Destroy(gameObject);
        }
        else if (!myDoor.isLocked)
        {
            Debug.LogWarning(string.Format("DoorKey target door {0} isn't locked, removing!", doorID));
            Destroy(gameObject);
        }
        else // Checks passed
        {
            // Make the key match the door's colour for consistency.
            GetComponent<SpriteRenderer>().color = myDoor.color;
        }
    }

    // On player collide, unlock the door and remove the key!
    public override void PlayerHit(Player player) {
        myDoor.Unlock();
        Destroy(gameObject);
    }
}
