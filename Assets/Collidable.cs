using UnityEngine;

public abstract class Collidable : MonoBehaviour {
    // These will be overriden by inheriting classes if they choose.
    public virtual void PlayerHit(Player player) { }
    public virtual void PlayerInteract(Player player) { }

    // Handler for collidables implementing triggers
    protected void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.activeTriggers.Add(gameObject);
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.activeTriggers.Remove(gameObject);
        }
    }

}