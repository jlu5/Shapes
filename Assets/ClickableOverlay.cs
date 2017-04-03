/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * ClickableOverlay.cs: Generic class for clickable canvas sprites.
 */
using UnityEngine;
using UnityEngine.UI;

public abstract class ClickableOverlay : MonoBehaviour
{
    protected Button button;

    protected virtual void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public abstract void OnClick();
}
