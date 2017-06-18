/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * PowerupGrow.cs: Implements the grow (size-up) powerup.
 */

public class PowerupGrow : Powerup {
    [Tooltip("Sets the size multiplier for this powerup.")]
    public float sizeMultiplier = 1.25F;
    [Tooltip("Sets the player mass multiplier for this powerup.")]
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
