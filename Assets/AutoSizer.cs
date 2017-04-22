using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSizer : MonoBehaviour {
    public float widthDecimal;
    public float offsetDecimal;
	
	void LateUpdate () {
        // Only run this section if we're active, and the necessary fields are filled in.
        RectTransform parentTransform = transform.parent.gameObject.GetComponent<RectTransform>();

        if (gameObject.activeInHierarchy && parentTransform && widthDecimal > 0F)
        {
            // Get the width of the parent canvas
            float totalwidth = parentTransform.rect.width;

            // Update the dimensions of the panel rectangle.
            RectTransform rt = GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2 (widthDecimal * totalwidth, 1);

            // Move the object a certain percentage from the edge of the screen (or object anchor).
            transform.position = new Vector3(offsetDecimal * totalwidth, transform.position.y, transform.position.z);
        }
	}
}
