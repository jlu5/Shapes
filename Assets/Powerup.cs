using UnityEngine;

public abstract class Powerup : Collidable {
    // How long does the powerup last?
    public float powerupLength = 5.0F;

    protected Player targetPlayer;

    // Function definitions for inheriting classes
    public abstract void SetEffect();
    public abstract void RemoveEffect();

    public override void PlayerHit(Player player)
    {

         // When a player hits the powerup, activate the powerup.
        targetPlayer = player;
        // Apply the effect
        SetEffect();
        // If the powerup length is > 0, set a timer to remove the timer later.
        if (powerupLength > 0.0F)
        {
            Invoke("RemoveEffect", powerupLength);
        }
        // Disable the renderer to act like the powerup has disappeared.
        // We don't use SetActive(false) or Destroy() because that would turn
        // off the scripts needed for RemoveEffect.
        Renderer renderer = gameObject.GetComponent<Renderer>();
        renderer.enabled = false;

        // Disable future collisions.
        Destroy(GetComponent<Collider2D>());
    }
}
