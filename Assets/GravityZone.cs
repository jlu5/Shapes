using UnityEngine;
using System.Collections.Generic;

public class GravityZone : Collidable {
    // How much should we multiple objects' gravity by as they enter?
    public float gravityMultiplier = 0.5f;

    // In case we want to cancel out gravity completely, keep track of objects' previous gravity
    // values - this is because we can't divide by 0 to restore the old value.
    //private Dictionary<GameObject, float> previousGravityScales = new Dictionary<GameObject, float>();

    void Start() {
        if (gravityMultiplier == 0)
        {
            Debug.LogError("Removing GravityZone with invalid gravity multiplier of 0");
            Destroy(this);
        }

        // Make sure the collider matches the size of the object after tiling.
        GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().size;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody)
        {
            // For gravity multipliers > 0, just multiply the gravity scale by the multiplier.
            Debug.Log("Object " + other.gameObject.name + " entered gravity zone");
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
