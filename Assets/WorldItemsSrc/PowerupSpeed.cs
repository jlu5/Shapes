/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * PowerupSpeed.cs: Implements the speed-up powerup.
 */

using UnityEngine;
using System.Collections.Generic;

public class PowerupSpeed : Powerup
{
    [Tooltip("Sets the speed multiplier for this powerup.")]
    public float speedMultiplier = 1.25F;

    public override void SetEffect()
    {
        targetPlayer.moveSpeed *= speedMultiplier;
        targetPlayer.gfx.SetSpeedGraphic(true);
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        targetPlayer.moveSpeed /= speedMultiplier;
        targetPlayer.gfx.SetSpeedGraphic(false);
    }
}
