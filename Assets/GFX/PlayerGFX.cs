/* Shapes Game (c) 2018 James Lu. All rights reserved.
 * PlayerGFX.cs: Implements various display elements related to Player.
 */


using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGFX : MonoBehaviour {
    private ParticleSystem ps;
    public Color color;  // should be synced to Player color
    private GameObject feet;  // Superjump powerup
    private GameObject speedGraphic;  // Speed powerup

    // Tracking for the rocket powerup's graphical effects.
    public const float idleModeParticlesMultiplier = 0.1F;
    private bool idleMode;

    void Awake () {
        ps = GetComponentInChildren<ParticleSystem>();
    }

    public void SetJumpGraphic(bool enabled) {
        feet.SetActive(enabled);
    }

    public void SetSpeedGraphic(bool enabled) {
        speedGraphic.SetActive(enabled);
    }

    public void SetRocketGraphic(bool enabled) {
        if (enabled) {
            ps.Play();
        } else {
            ps.Stop();
        }
    }

    public void SetRocketEmission(float movement) {
        // Set the player's particle system to emit fewer or more particles based on whether
        // the rocket is enabled.
        ParticleSystem.EmissionModule emission = ps.emission;
        if (movement == 0 && !idleMode)
        {
            emission.rateOverTimeMultiplier *= idleModeParticlesMultiplier;
            Debug.Log("emission.rateOverTimeMultiplier after idleMode enable: " + emission.rateOverTimeMultiplier.ToString());
            idleMode = true;
        }
        else if (idleMode)
        {
            emission.rateOverTimeMultiplier /= idleModeParticlesMultiplier;
            Debug.Log("emission.rateOverTimeMultiplier after idleMode disable: " + emission.rateOverTimeMultiplier.ToString());
            idleMode = false;
        }
    }

    void Start () {
        // Set the particle system to match the player color.
        ParticleSystem.MainModule psmain = ps.main;
        // Use a gradient that slowly fades from the player color to emptiness.
        psmain.startColor = new ParticleSystem.MinMaxGradient(color, new Color(color.r, color.g, color.b, 0));

        // The "feet" graphic is used to show the superjump powerup.
        feet = transform.Find("PlayerFeet").gameObject;
        feet.GetComponent<SpriteRenderer>().color = color;
        // Make it also match the player color.
        feet.SetActive(false);

        speedGraphic = transform.Find("PowerupSpeedEffect").gameObject;
    }
}
