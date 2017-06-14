/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * GameBoundary.cs: Creates boundaries for the level, destroying all objects that go past its bounds.
 */

using UnityEngine;

public class GameBoundary : MonoBehaviour {
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Note: Only destroy objects that are *actually* leaving the area of the stage.
        // In other words, ignore objects that disable their collider intentionally (e.g. powerups when hit).
        Debug.Log("other position: " + other.transform.position.ToString());
        Debug.Log("overlap check: " + col.OverlapPoint(other.transform.position).ToString());
        if (other.attachedRigidbody && !col.OverlapPoint(other.transform.position))
        {
            // For most objects, destroy them completely.
            Debug.Log("GameBoundary: removing object " + other.gameObject.name);
            Destroy(other.gameObject);
        }
    }
}
