public class NextLevelButton : ClickableOverlay
{
    public override void OnClick()
    {
        if (LevelSelector.Instance != null)
        {
            LevelSelector.Instance.PlayNextLevel();
        }
    }
}
