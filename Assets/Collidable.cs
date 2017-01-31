using System;
using UnityEngine;

public abstract class Collidable : MonoBehaviour {
    // This will be overriden by inheriting classes.
    public abstract void PlayerHit(Player player);
/*
    // This method passes player hit calls to the class representing
    // an object, based on its tag.
    void PlayerHit(Player player)
    {
        if (string.IsNullOrEmpty(tag)) // No tag on the object; break
        {
            return;
        }
        Type type = Type.GetType(tag);
        if (type == null) // Unknown tag, return.
        {
            Debug.LogWarning(string.Format("Collidable got null type from tag {0}", tag));
            return;
        }
        dynamic instance = GetComponent(type);
        if (instance != null)
        {
            instance.PlayerHit(player);
        }
    }
*/
}
