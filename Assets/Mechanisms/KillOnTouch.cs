/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * KillOnTouch.cs: Creates "deadly" objects that kill players on contact.
 */

using UnityEngine;

public class KillOnTouch : Collidable {

    public override void PlayerHit(Player player)
    {
        // Kill the player on hit!
        Destroy(player.gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // Kill players as they enter the object, for trigger-based
        // objects like lava.
        if (other.gameObject.GetComponent<Player>())
            Destroy(other.gameObject);
    }
}
