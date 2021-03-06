﻿/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * LevelSelectButton.cs: Implements the buttons used in the level selector to load levels.
 */

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
            LevelSelector.Instance.SwitchCanvas(OverlayCanvas.NONE);

            // Set the level position.
            LevelSelector.Instance.lastLevel = levelNum;
            Debug.Log("Trying to load scene " + levelName);
            SceneManager.LoadScene(levelName);
            Scene scene = SceneManager.GetSceneByName(levelName);
            SceneManager.SetActiveScene(scene);

            // Update the level name in the Game Paused text
            Utils.SetText(LevelSelector.Instance.gamePausedText, string.Format("Game Paused (level: {0})", scene.name));
        }
    }
}
