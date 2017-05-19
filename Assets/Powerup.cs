using UnityEngine;
using UnityEngine.UI;

public abstract class Powerup : Collidable {
    // How long does the powerup last?
    public float powerupLength = 5.0F;

    protected Player targetPlayer;
    protected GameObject powerupDisplay;

    // Function definitions for inheriting classes
    public abstract void SetEffect();
    public virtual void RemoveEffect()
    {
        // Remove the powerup display.
        Destroy(powerupDisplay);
    }

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
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        renderer.enabled = false;

        // Show a copy of the powerup in the gamestate's powerups panel.
        powerupDisplay = new GameObject();
        powerupDisplay.transform.SetParent(GameState.Instance.powerupsPanel.transform);
        Image image = powerupDisplay.AddComponent<Image>();
        // Give the powerup display the same sprite as the current object, and set the size to 32x32.
        image.sprite = renderer.sprite;
        image.rectTransform.sizeDelta = new Vector2(32, 32);

        // Disable future collisions.
        Destroy(GetComponent<Collider2D>());
    }
}
