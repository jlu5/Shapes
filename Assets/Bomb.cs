using UnityEngine;

public class Bomb : Collidable
{
    public float explosionRadius = 3.0f;
    public float explosionForce = 5f;

    public void Explode()
    {
        // Basically what we need to do is find a list of objects close to the bomb when it explodes,
        // and propel them away. The closer the object is to the bomb, the more force is applied on it.
        Collider2D[] colliderResults = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collider in colliderResults)
        {
            if (collider.attachedRigidbody)
            {
                // Calculate the difference between the target object and the bomb's positions.
                Vector2 differenceVector = collider.transform.position - transform.position;
                float distance = differenceVector.magnitude;

                // Calculate a force amount inversely proportional to the distance betwen the two objects.
                float force = Mathf.Min(1 / distance, distance) * explosionForce;

                // Fetch the angle between the bomb and the target object.
                float explosionAngle = Mathf.Atan2(differenceVector.y, differenceVector.x);

                // Create a new vector based off this angle.
                Vector2 targetVector = new Vector2(force * Mathf.Cos(explosionAngle), force * Mathf.Sin(explosionAngle));

                collider.attachedRigidbody.AddForce(targetVector, ForceMode2D.Impulse);

                Debug.Log("Difference from object " + collider.gameObject.name + " to bomb: " + differenceVector.ToString());
                Debug.Log("Explosion angle is " + explosionAngle + " radians with object " + collider.gameObject.name);
                Debug.Log("cos(" + explosionAngle.ToString() + ") is " + Mathf.Cos(explosionAngle).ToString());
                Debug.Log("Using force " + force.ToString() + " on object " + collider.gameObject.name);
                Debug.Log("Adding force of magnitude " + targetVector.ToString() + " to object " + collider.gameObject.name);
            }
        }
    }

    public override void PlayerHit(Player player)
    {
        Explode();
        Destroy(gameObject);
    }
}
