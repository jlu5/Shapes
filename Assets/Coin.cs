/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Coin.cs: Provides a coin object that increments the score when taken.
 */

using UnityEngine;

public class Coin : Collidable {
    [Tooltip("How much is this coin worth?")]
    public int value = 100;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // When a player hits the coin, increment the score and destroy the coin.
        if (other.gameObject.GetComponent<Player>() != null)
        {
            // Add to the score and coin count
            GameState.Instance.addCoin(value);

            // Remove the coin; they're single use.
            Destroy(gameObject);
        }
    }
}
