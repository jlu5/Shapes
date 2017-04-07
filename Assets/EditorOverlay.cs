/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * PlayerOverlay.cs: Implements clickable player sprites for HUDCanvas instances
 */
using UnityEngine;
using System;

public class EditorOverlay : ClickableOverlay
{
    public string resourceName;

    // When clicked, set the active editor object to this.
    public override void OnClick()
    {
        if (string.IsNullOrEmpty(resourceName))
        {
            throw new InvalidOperationException("Resource name not set for EditorOverlay object");
        }

        Debug.Log("Setting current object to " + resourceName);
        Editor.Instance.currentObject = resourceName;
    }
}
