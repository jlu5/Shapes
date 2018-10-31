/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * PowerupJump.cs: Implements the superjump powerup.
 */

using UnityEngine;

public class PowerupJump : Powerup {
    [Tooltip("Sets the jump height multiplier for this powerup.")]
    public float jumpMultiplier = 1.25F;

    public override void SetEffect()
    {
        // TODO: consider changing jumpRecoilStrength as well?
        targetPlayer.jumpStrength *= jumpMultiplier;
        // Show the "feet" graphic on the player.
        targetPlayer.gfx.SetJumpGraphic(true);
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        targetPlayer.jumpStrength /= jumpMultiplier;
        targetPlayer.gfx.SetJumpGraphic(false);
    }
}
