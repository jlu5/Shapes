/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Bomb.cs: Creates an in-game bomb object that explodes on player contact.
 */

using UnityEngine;

public class Bomb : Collidable
{
    [Tooltip("Sets the explosion radius of this bomb.")]
    public float explosionRadius = 3f;
    [Tooltip("Sets the max explosion force of this bomb: the explosion force applied on each object is inversely proportional to the distance between the bomb and the affected object, but capped at this value.")]
    public float explosionForce = 5f;

    public override void PlayerHit(Player player)
    {
        Utils.AddExplosionForce2D(gameObject, explosionRadius, explosionForce);
        Destroy(gameObject);
    }
}
