using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButton : ClickableOverlay
{
    public string levelName { get; set; }

	public override void OnClick()
    {
        if (!string.IsNullOrEmpty(levelName))
        {
            Debug.Log("Trying to load scene " + levelName);
            SceneManager.LoadScene(levelName);
            Scene scene = SceneManager.GetSceneByName(levelName);
            SceneManager.SetActiveScene(scene);
        }
    }
}
