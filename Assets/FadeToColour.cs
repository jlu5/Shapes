using UnityEngine;
using UnityEngine.UI;

public class FadeToColour : MonoBehaviour {
    // This is a quick hack because a null UI.Image causes nothing but a solid colour to be drawn...
    Image image;
    Color color;
    public float fadeAmountPerFrame = 0.005F;

	void Start () {
        image = GetComponent<Image>();
        color = image.color;
	}

	void Update () {
        // Every frame, add to the image's alpha (transparency) value until it becomes
        // completely opaque. In the context of alpha values, 0 = completely transparent
        // (this object's default), and 1 = completely opaque.
		if (color.a < 1)
        {
            color.a += fadeAmountPerFrame;
            // Replace the image colour with the one we're tracking internally
            image.color = color;
        }
	}
}
