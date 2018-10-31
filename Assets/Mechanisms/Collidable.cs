/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Collidable.cs: Base class implementing hooks for player collision and interaction.
 */
using UnityEngine;

public abstract class Collidable : MonoBehaviour {
    // These will be overriden by inheriting classes if they choose.
    public virtual void PlayerHit(Player player) { }
    public virtual void PlayerInteract(Player player) { }

    // For go-through collidables implementing triggers, these methods implement
    // tracking for which triggers a player is colliding with at any given time.
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Try to fetch the player script of the triggering object, if it exists.
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.activeTriggers.Add(gameObject);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.activeTriggers.Remove(gameObject);
        }
    }

}