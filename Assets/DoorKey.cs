using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKey : Collidable {
    public int doorID;
    private Door myDoor;

    // Note: this start method requires the "Door" script to be executed before it! (Unity Script Execution Order)
    void Start() {
        // Note: (Door) explicitly casts this from Collidable to Door. XXX: look into whether it's worth
        // making GetCollidable return the specific item type itself.
        myDoor = (Door) GameState.Instance.GetCollidable<Door>(doorID);

        // Sanity check to make sure the existance of this key makes sense.
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
        else
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

    // PlayerInteract is a stub.
    public override void PlayerInteract(Player player) {
    }
}
