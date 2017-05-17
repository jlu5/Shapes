using UnityEngine.SceneManagement;

public class OpenLevelSelectorButton : ClickableOverlay
{
    public override void OnClick()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
