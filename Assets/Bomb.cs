/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Bomb.cs: Creates an in-game bomb object that explodes on player contact.
 */

using UnityEngine;

public class Bomb : Collidable
{
    public float explosionRadius = 3.0f;
    public float explosionForce = 5f;

    public override void PlayerHit(Player player)
    {
        Utils.AddExplosionForce2D(gameObject, explosionRadius, explosionForce);
        Destroy(gameObject);
    }
}
