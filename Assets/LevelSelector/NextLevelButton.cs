/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * NextLevelButton.cs: Implements the button to move to the next level.
 */

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
