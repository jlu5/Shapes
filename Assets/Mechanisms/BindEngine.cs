/* Shapes Game (c) 2018 James Lu. All rights reserved.
 * BindEngine.cs: Implements player binding.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BindEngine : MonoBehaviour {
    private List<RelativeJoint2D> bindJoints = new List<RelativeJoint2D>();
    private List<GameObject> bindDisplays = new List<GameObject>();
    protected List<BindEngine> boundObjects = new List<BindEngine>();
    private GameObject bindDisplayTemplate; // BindDisplayObject template

    // Attempts to attach to all colliding players.
    public void Attach()
    {
        Rigidbody2D rb;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        Collider2D[] collisions = new Collider2D[Player.MaxCollisionCount];
        rb.GetContacts(collisions);

		// Check each colliding collider
        foreach (Collider2D collider in collisions)
        {
            if (collider == null)
            {
                continue;
            }

			// If it has a BindEngine instance, try to bind!
            GameObject otherObject = collider.gameObject;
            BindEngine target = otherObject.GetComponent<BindEngine>();
            if (target != null)
            {
                Bind(target);
            }
        }
    }

    void Awake() {
        bindDisplayTemplate = Resources.Load<GameObject>("BindDisplayObject");
    }

    // Detaches from currently attached players.
    public void Detach()
    {
		// Remove associations to other BindEngine instances
        if (boundObjects.Any()) {
			for (int i = boundObjects.Count-1; i >= 0; i--) {
				BindEngine other = boundObjects[i];
				Debug.Log(string.Format("BindEngine: unbinding objects {0} and {1}", gameObject.name, other.gameObject.name));
				other.boundObjects.Remove(this);
				boundObjects.RemoveAt(i);
			}
        }

        // Remove all bind displays and joints. This assumes that bindJoints and bindDisplays always
        // have the same lengths, since no other code explicitly removes them even if an object in
        // the list is destroyed.
        for (int i = bindJoints.Count-1; i >= 0; i--) {
            Joint2D joint = bindJoints[i];
            if (joint != null) {
                Destroy(joint);
            }
            bindJoints.RemoveAt(i);

            GameObject bd = bindDisplays[i];
            if (bd != null) {
                Destroy(bd);
            }
            bindDisplays.RemoveAt(i);
        }
    }

    private void CreateBindDisplay(BindEngine target) {
        // Create a new bind display object from our prefab, if one doesn't exist already.
        GameObject bdo = Instantiate(bindDisplayTemplate);

        bdo.transform.SetParent(transform);

        // Set the bind display script to track this character and the other one.
        BindDisplay bdoScript = bdo.GetComponent<BindDisplay>();
        bdoScript.object1 = gameObject;
        bdoScript.object2 = target.gameObject;

        bindDisplays.Add(bdo);
        target.bindDisplays.Add(bdo);
    }

    public void Bind(BindEngine target) {
        if (boundObjects.Contains(target)) {
            return;
        };
        // Create a relative (angle and distance preserving) joint between the two objects.
        RelativeJoint2D joint = gameObject.AddComponent<RelativeJoint2D>();
        joint.connectedBody = target.gameObject.GetComponent<Rigidbody2D>();
        joint.enableCollision = false;
        Debug.Log(string.Format("BindEngine: binding objects {0} and {1}", gameObject.name, target.gameObject.name));

        bindJoints.Add(joint);
        target.bindJoints.Add(joint);

        boundObjects.Add(target);
        target.boundObjects.Add(this);

        CreateBindDisplay(target);
    }
}
