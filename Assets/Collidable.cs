using UnityEngine;

public abstract class Collidable : MonoBehaviour {
    // This will be overriden by inheriting classes.
    public abstract void PlayerHit(Player player);
}
