using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerButton : Collidable {
    [Tooltip("Sets whether the trigger is currently on.")]
    public bool isOn;

    // List of sprites to use (first sprite AKA index 0 = off, second sprite AKA index 1 = on)
    public Sprite[] sprites = new Sprite[2];

    protected SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[isOn ? 1 : 0];  // Set the initial sprite based on the default isOn value.
    }

	public override void PlayerInteract(Player player) {
        isOn = !isOn;  // Flip the isOn bool.
        spriteRenderer.sprite = sprites[isOn ? 1 : 0];
	}
}
