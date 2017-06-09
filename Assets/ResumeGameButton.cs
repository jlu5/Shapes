using UnityEngine;

public class ResumeGameButton : ClickableOverlay
{
    public override void OnClick()
    {
        Time.timeScale = GameState.Instance.timeScale;
        LevelSelector.Instance.SwitchCanvas(-1);  // Hide the navigation menu
    }
}
