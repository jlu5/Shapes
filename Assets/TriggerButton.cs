using UnityEngine;

public class TriggerButton : Collidable {
    [Tooltip("Sets whether the trigger is currently on.")]
    public bool isOn;

    [Tooltip("Sets the target AutoMover ID to trigger.")]
    public int targetID;

    // List of sprites to use (first sprite AKA index 0 = off, second sprite AKA index 1 = on)
    public Sprite[] sprites = new Sprite[2];

    // Access to attributes.
    protected SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[isOn ? 1 : 0];  // Set the initial sprite based on the default isOn value.
        UpdateAnimation();  // Play/stop the animation based on the button default
    }

    void UpdateAnimation()
    {
        AutoMover target = (AutoMover)GameState.Instance.GetGameScript<AutoMover>(targetID);
        if (!target)
        {
            Debug.LogError(string.Format("TriggerButton: No target AutoMover object with ID {0} found!", targetID));
            return;
        }

        // Pause/unpause the target AutoMover animation by setting the speed.
        target.anim["AutoMover"].speed = isOn ? 1 : 0;
    }

	public override void PlayerInteract(Player player) {
        isOn = !isOn;  // Flip the isOn bool.
        spriteRenderer.sprite = sprites[isOn ? 1 : 0];
        UpdateAnimation();
    }
}
