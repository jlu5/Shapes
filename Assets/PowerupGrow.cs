public class PowerupGrow : Powerup {
    public float sizeMultiplier = 1.25F;
    public float massMultiplier = 2F;

    public override void SetEffect()
    {
        targetPlayer.gameObject.transform.localScale *= sizeMultiplier;
        targetPlayer.rb.mass *= massMultiplier;
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        targetPlayer.gameObject.transform.localScale /= sizeMultiplier;
        targetPlayer.rb.mass /= massMultiplier;
    }
}
