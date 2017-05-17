public class RestartLevelButton : ClickableOverlay {

    public override void OnClick()
    {
        GameState.Instance.RestartLevel();
    }

}
