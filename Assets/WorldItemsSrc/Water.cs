/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Water.cs: Implements water with support for player swimming.
 */

using UnityEngine;

public class Water : Collidable {
    [Tooltip("What swimming speed should we add to the player when underwater?")]
    public float swimSpeed = 0.02F;

    [Tooltip("Should swimming in water be allowed?")]
    public bool allowSwim = true;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player)
        {
            if (allowSwim)
            {
                // Let the player swim (this actually borrows the flying code).
                player.canFly++;
                player.flySpeed += swimSpeed;
            }
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player && allowSwim)
        {
            // Remove the ability to fly (swim) if the player leaves the water.
            player.canFly--;
            player.flySpeed -= swimSpeed;
        }
    }
}
