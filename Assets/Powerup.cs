using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class Powerup : Collidable {
    // How long does the powerup last?
    public float powerupLength = 5.0F;

    // Text colors for the "time remaining" overlay
    public static Color textColor = new Color(0, 0, 0, 0.5f);
    public static Color warningTextColor = new Color(0.8f, 0, 0, 0.5f);

    // If the time remaining is under this amount, switch the "time remaining" overlay to warningTextColor.
    public float warningThreshold = 1.0F;

    // Access to other subobjects and the owning player
    protected Player targetPlayer;
    protected GameObject powerupDisplay;
    protected GameObject powerupRemainingTextBox;

    // Track powerup state and time remaining
    protected float startTime;
    protected bool isUsed;

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

        // Mark the powerup as used.
        isUsed = true;

        // Disable the renderer to act like the powerup has disappeared.
        // We don't use SetActive(false) or Destroy() because that would turn
        // off the scripts needed for RemoveEffect.
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        renderer.enabled = false;

        // Show a copy of the powerup in the gamestate's powerups panel.
        powerupDisplay = new GameObject();
        powerupDisplay.transform.SetParent(GameState.Instance.powerupsPanel.transform);
        Image image = powerupDisplay.AddComponent<Image>();
        // Give the powerup display the same sprite as the current object, and set the size to 48x48.
        image.sprite = renderer.sprite;
        image.rectTransform.sizeDelta = new Vector2(48, 48);
        // Make the overlay colour correspond to the player color.
        image.color = player.getColor();

        // If the powerup length is > 0, set a timer to remove the timer later.
        if (powerupLength > 0)
        {
            Invoke("RemoveEffect", powerupLength);

            // Add a text box in the powerup display to show the time remaining.
            powerupRemainingTextBox = Instantiate(GameState.Instance.textLabelTemplate, powerupDisplay.transform);
            powerupRemainingTextBox.transform.position = Vector3.zero;
            Text text = powerupRemainingTextBox.GetComponent<Text>();
            text.color = textColor;
            text.fontStyle = FontStyle.Bold;  // Make the text bold for better readability
        }
        startTime = Time.time;

        // Allow clicking on the powerup display to focus on the player.
        PlayerOverlay po = powerupDisplay.AddComponent<PlayerOverlay>();
        po.playerID = player.playerID;
        po.showPlayerID = false; // The player ID text clashes with the "time remaining" display, so turn it off.

        // Disable future collisions.
        Destroy(GetComponent<Collider2D>());
    }

    protected virtual void FixedUpdate() {

        if (isUsed && !targetPlayer)
        {
            // The player we're attached to disappeared, so remove the powerup.
            Destroy(powerupDisplay);
            Destroy(gameObject);
        }

        if (powerupLength > 0)
        {
            // Update the power up display text to show how much time is remaining in the powerup.
            // "F1" in ToString format the float with one decimal place.
            float timeRemaining = startTime + powerupLength - Time.time;
            Utils.SetText(powerupRemainingTextBox, (timeRemaining).ToString("F1"));
            // Warn the player by the text color red if there's little time remaining.
            if (timeRemaining < warningThreshold)
            {
                try
                {
                    Text text = powerupRemainingTextBox.GetComponent<Text>();
                    text.color = warningTextColor;
                }
                catch (NullReferenceException)
                {
                    // Ignore errors if the text box got deleted while this function is running.MissingReferenceException
                }
                catch (MissingReferenceException)
                {
                }
            }
        }
    }
}
