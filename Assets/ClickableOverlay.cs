/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * ClickableOverlay.cs: Generic base class for clickable canvas sprites.
 */
using UnityEngine;
using UnityEngine.UI;

public abstract class ClickableOverlay : MonoBehaviour
{
    protected Button button;

    protected virtual void Start()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            // No button exists, so make a new one
            button = gameObject.AddComponent<Button>();
        }
        button.onClick.AddListener(OnClick);
    }

    public abstract void OnClick();
}
