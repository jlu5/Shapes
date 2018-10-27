/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * RestartLevelButton.cs: Implements buttons to restart the current level.
 */

public class RestartLevelButton : ClickableOverlay {

    public override void OnClick()
    {
        GameState.Instance.RestartLevel();
        LevelSelector.Instance.SwitchCanvas(OverlayCanvas.NONE);  // Hide the navigation menu
    }
}
