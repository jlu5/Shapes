/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * OpenLevelSelectorButton.cs: Implements the navigation button to return back to the main scene.
 */

using UnityEngine.SceneManagement;

public class OpenLevelSelectorButton : ClickableOverlay
{
    public override void OnClick()
    {
        SceneManager.LoadScene("MainScene");
    }
}
