using UnityEngine;

public class PowerupRocket : Powerup {
    // Calculate the power of the rocket by multiplying acceleration due to gravity by this multiplier.
    // Compared to hardcoding a force value, this relative calculation ensures that the rocket speed isn't insanely fast.
    public float rocketSpeed = 0.05F;
    public bool rocketOn;

    public override void SetEffect()
    {
        rocketOn = true;
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        rocketOn = false;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        // If the rocket is enabled and the player with the rocket is selected, let the player fly!
        // TODO: perhaps add optional fuel amounts so that using the rocket too much will destroy it?
        if (rocketOn && targetPlayer.playerID == GameState.Instance.currentPlayer) {
            float movement = Input.GetAxis("PowerupMove");
            // Calculate a target force based off the gravity scale.
            Vector2 force = rocketSpeed * -Physics2D.gravity * targetPlayer.rb.gravityScale;
            Debug.Log("PowerupRocket: moving by " + (movement * force).ToString());
            targetPlayer.rb.AddForce(force*movement, ForceMode2D.Impulse);
        }
    }
}