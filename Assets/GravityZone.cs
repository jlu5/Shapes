/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * GravityZone.cs: Implements gravity zones that change gravity controls when entering.
 */

using UnityEngine;
using System.Collections.Generic;

public class GravityZone : Collidable {
    // How much should we multiple objects' gravity by as they enter?
    public float gravityMultiplier = 0.5f;

    void Start() {
        // Make sure the collider matches the size of the object after tiling.
        GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().size;

        if (gravityMultiplier == 0)
        {
            // We can't divide by zero, so set the multiplier to a really low value to simulate lack of gravity.
            gravityMultiplier = -1e-32f;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody)
        {
            // For gravity multipliers > 0, just multiply the gravity scale by the multiplier.
            Debug.Log("Object " + other.gameObject.name + " entered gravity zone");

            // Multiply the target object's gravity scale by the multiplier.
            other.attachedRigidbody.gravityScale *= gravityMultiplier;
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody)
        {
            Debug.Log("Object " + other.gameObject.name + " left gravity zone");
            other.attachedRigidbody.gravityScale /= gravityMultiplier;
        }
    }
}
