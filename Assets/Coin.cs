using UnityEngine;

public class Coin : Collidable {
    // How much is this coin worth?
    public int value = 100;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // When a player hits the coin, increment the score and destroy the coin.
        if (other.gameObject.GetComponent<Player>() != null)
        {
            GameState.Instance.AddScore(value);
            Destroy(gameObject);
        }
    }
}
