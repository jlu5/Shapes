public class RestartLevelButton : ClickableOverlay {

    public override void OnClick()
    {
        GameState.Instance.RestartLevel();
        LevelSelector.Instance.SwitchCanvas(-1);  // Hide the navigation menu
    }
}