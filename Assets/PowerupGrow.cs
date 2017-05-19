public class PowerupGrow : Powerup {
    public float sizeMultiplier = 1.25F;

    public override void SetEffect()
    {
        targetPlayer.gameObject.transform.localScale *= sizeMultiplier;
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        targetPlayer.gameObject.transform.localScale /= sizeMultiplier;
    }
}
