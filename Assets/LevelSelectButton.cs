using UnityEngine.SceneManagement;

public class LevelSelectButton : ClickableOverlay
{
    public string path;

	public override void OnClick()
    {
        if (!string.IsNullOrEmpty(path))
        {
            Scene scene = SceneManager.GetSceneByPath(path);
            SceneManager.SetActiveScene(scene);
        }
    }
}
