using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButton : ClickableOverlay
{
    public string levelName { get; set; }
    public int levelNum { get; set; }

	public override void OnClick()
    {
        if (!string.IsNullOrEmpty(levelName))
        {
            // Hide all the level selection canvases.
            LevelSelector.Instance.SwitchCanvas(-1);

            // Set the level position.
            LevelSelector.Instance.lastLevel = levelNum;
            Debug.Log("Trying to load scene " + levelName);
            SceneManager.LoadScene(levelName);
            Scene scene = SceneManager.GetSceneByName(levelName);
            SceneManager.SetActiveScene(scene);
        }
    }
}
