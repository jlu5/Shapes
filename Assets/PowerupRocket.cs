using UnityEngine;

public class PowerupRocket : Powerup {
    // Calculate the power of the rocket by multiplying acceleration due to gravity by this multiplier.
    // Compared to hardcoding a force value, this relative calculation ensures that the rocket speed isn't insanely fast.
    public float rocketSpeed = 0.05F;
    public bool rocketOn;

    public float idleModeParticlesMultiplier = 0.1F;
    private bool idleMode;

    public override void SetEffect()
    {
        rocketOn = true;
        // Turn on the player object's particle system.
        targetPlayer.ps.Play();
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        rocketOn = false;
        targetPlayer.ps.Stop();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        // If the rocket is enabled and the player with the rocket is selected, let the player fly!
        // TODO: perhaps add optional fuel amounts so that using the rocket too much will destroy it?
        if (rocketOn && targetPlayer.playerID == GameState.Instance.currentPlayer) {
            float movement = Input.GetAxis("PowerupMove");

            // Calculate a target force based off a portion of the current gravity.
            Vector2 force = rocketSpeed * -Physics2D.gravity * targetPlayer.rb.gravityScale * targetPlayer.rb.mass;

            Debug.Log("PowerupRocket: moving by " + (movement * force).ToString());
            targetPlayer.rb.AddForce(force*movement, ForceMode2D.Impulse);

            // Set the player's particle system to emit fewer or more particles based on whether
            // the rocket is enabled.
            ParticleSystem.EmissionModule emission = targetPlayer.ps.emission;
            if (movement == 0 && !idleMode)
            {
                emission.rateOverTimeMultiplier *= idleModeParticlesMultiplier;
                Debug.Log("emission.rateOverTimeMultiplier after idleMode enable: " + emission.rateOverTimeMultiplier.ToString());
                idleMode = true;
            }
            else if (idleMode)
            {
                emission.rateOverTimeMultiplier /= idleModeParticlesMultiplier;
                Debug.Log("emission.rateOverTimeMultiplier after idleMode disable: " + emission.rateOverTimeMultiplier.ToString());
                idleMode = false;
            }
        }
    }
}