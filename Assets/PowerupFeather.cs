/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * PowerupFeather.cs: Implements the feather (low gravity) powerup.
 */

public class PowerupFeather : Powerup
{
    public float gravityMultiplier = 0.5F;

    public override void SetEffect()
    {
        targetPlayer.rb.gravityScale *= gravityMultiplier;
        // TODO: show some graphic?
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        targetPlayer.rb.gravityScale /= gravityMultiplier;
    }
}
