/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * PowerupSpeed.cs: Implements the speed-up powerup.
 */

using UnityEngine;
using System.Collections.Generic;

public class PowerupSpeed : Powerup
{
    public float speedMultiplier = 1.25F;
    private GameObject effectTemplate;
    private List<GameObject> spheres = new List<GameObject>();

    void Start() {
        effectTemplate = Resources.Load<GameObject>("PowerupSpeedEffect");
    }

    void AddSphere() {
        spheres.Add(Instantiate(effectTemplate, targetPlayer.spheresContainer.transform));
    }

    public override void SetEffect()
    {
        targetPlayer.moveSpeed *= speedMultiplier;

        // Spawn 1 to 3 spheres that circle around the player; this is used to display the speed powerup.
        for (int i=0; i < (int) Random.Range(1, 3); i++) {
            // Delay each spawn by up to 0.3 secs, to vary up the initial positions of the spheres.
            Invoke("AddSphere", Random.Range(0f, System.Math.Min(0.3f, powerupLength/2)));
        }
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        targetPlayer.moveSpeed /= speedMultiplier;

        // Delete all the spheres that we made.
        foreach (GameObject sphere in spheres.ToArray())
        {
            spheres.Remove(sphere);
            Destroy(sphere);
        }
    }
}
