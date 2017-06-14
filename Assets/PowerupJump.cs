/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * PowerupJump.cs: Implements the superjump powerup.
 */

public class PowerupJump : Powerup {
    public float jumpMultiplier = 1.25F;

    public override void SetEffect()
    {
        // TODO: consider changing jumpRecoilStrength as well?
        targetPlayer.jumpStrength *= jumpMultiplier;
        // Show the "feet" graphic on the player.
        targetPlayer.feet.SetActive(true);
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        targetPlayer.jumpStrength /= jumpMultiplier;
        targetPlayer.feet.SetActive(false);
    }
}
