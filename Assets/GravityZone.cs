using UnityEngine;
using System.Collections.Generic;

public class GravityZone : Collidable {
    // How much should we multiple objects' gravity by as they enter?
    public float gravityMultiplier = 0.5f;

    // In case we want to cancel out gravity completely, keep track of objects' previous gravity
    // values - this is because we can't divide by 0 to restore the old value.
    //private Dictionary<GameObject, float> previousGravityScales = new Dictionary<GameObject, float>();

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

            /*
            // If the gravity multiplier is 0, we have to use a different method of storing objects' last gravity
            // scale as they enter and leave the area.
            if (!previousGravityScales.ContainsKey(other.gameObject))
            {
                previousGravityScales[other.gameObject] = other.attachedRigidbody.gravityScale;
            }
            */

            // Multiply the target object's gravity scale by the multiplier.
            other.attachedRigidbody.gravityScale *= gravityMultiplier;
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody)
        {
            Debug.Log("Object " + other.gameObject.name + " left gravity zone");

            /*
            if (other.attachedRigidbody.gravityScale == 0 || gravityMultiplier == 0)
            {
                // Reset the target object from 0 gravity to its original gravity scale.
                other.attachedRigidbody.gravityScale = defaultGravityScale;
            }
            else
            {
                // For multipliers != 0, we can just divide by the multiplier.
                other.attachedRigidbody.gravityScale /= gravityMultiplier;
            }
            */
            other.attachedRigidbody.gravityScale /= gravityMultiplier;
        }
        //previousGravityScales.Remove(other.gameObject);
    }
}
