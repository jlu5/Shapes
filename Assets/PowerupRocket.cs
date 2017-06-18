﻿/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * PowerupRocket.cs: Implements the rocket powerup.
 */

using UnityEngine;

public class PowerupRocket : Powerup {
    [Tooltip("Sets the speed of this rocket.")]
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
