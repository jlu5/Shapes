public class PowerupJump : Powerup {
    public float jumpMultiplier = 1.25F;

    public override void SetEffect()
    {
        // TODO: consider changing jumpRecoilStrength as well?
        targetPlayer.jumpStrength *= jumpMultiplier;
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        targetPlayer.jumpStrength /= jumpMultiplier;
    }
}
