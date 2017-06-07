using UnityEngine;

public class GameBoundary : MonoBehaviour {
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player)
        {
            // Destroy players via GameState, unregistering their overlay etc.
            Debug.Log("GameBoundary: removing player " + player.playerID);
            GameState.Instance.RemovePlayer(player.playerID);
        }

        // Note: Only destroy objects that are *actually* leaving the area of the stage.
        // In other words, ignore objects that disable their collider intentionally (e.g. powerups when hit).
        if (other.attachedRigidbody && !col.OverlapPoint(other.transform.position))
        {
            // For most objects, destroy them completely.
            Debug.Log("GameBoundary: removing object " + other.gameObject.name);
            Destroy(other.gameObject);
        }
    }
}
