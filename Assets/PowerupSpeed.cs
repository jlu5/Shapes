public class PowerupSpeed : Powerup
{
    public float speedMultiplier = 1.25F;

    public override void SetEffect()
    {
        targetPlayer.moveSpeed *= speedMultiplier;
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        targetPlayer.moveSpeed /= speedMultiplier;
    }
}
