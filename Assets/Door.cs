using UnityEngine;

public class Door : Collidable {
    public int ID;
    public int targetDoor;

    void Start()
    {
        if (targetDoor == ID)
        {
            Debug.LogError(string.Format("Door {0} has target of itself; removing!", ID));
            Destroy(gameObject);
        }
        GameState.Instance.registerCollidable(ID, this);
    }
    
    // PlayerHit is a no-op
	public override void PlayerHit (Player player) {
	}

    public override void PlayerInteract(Player player)
    {
        Collidable otherDoor = GameState.Instance.getCollidable<Door>(targetDoor, true);
        if (otherDoor == null)
        {
            // XXX make this obvious to the player outside the editor
            Debug.LogWarning(string.Format("This door is locked! (points to invalid door {0})", targetDoor));
        } else
        {
            // Teleport the player!
            player.gameObject.transform.position = otherDoor.gameObject.transform.position;
        }
    }


}
