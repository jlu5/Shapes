/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * ResumeLevelButton.cs: Implements the button to resume the game/current level.
 */

using UnityEngine;

public class ResumeGameButton : ClickableOverlay
{
    public override void OnClick()
    {
        Time.timeScale = GameState.Instance.timeScale;
        LevelSelector.Instance.SwitchCanvas(OverlayCanvas.NONE);  // Hide the navigation menu
    }
}
