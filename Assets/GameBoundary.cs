using UnityEngine;

public class GameBoundary : MonoBehaviour {

    void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player)
        {
            // Destroy players properly
            GameState.Instance.RemovePlayer(player.playerID);
        }

        if (other.attachedRigidbody)
        {
            // For most objects, destroy them completely.
            Destroy(other.gameObject);
        }
    }
}
