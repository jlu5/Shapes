using UnityEngine;

public class PowerupRocket : Powerup {
    public float rocketSpeed = 0.03F;

    public override void SetEffect()
    {
        targetPlayer.canFly += 1;
        // Turn on the player object's particle system.
        targetPlayer.ps.Play();
        targetPlayer.flySpeed += rocketSpeed;
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        targetPlayer.canFly -= 1;
        targetPlayer.ps.Stop();
        targetPlayer.flySpeed -= rocketSpeed;
    }
}