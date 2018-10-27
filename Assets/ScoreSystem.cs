/* Shapes Game (c) 2018 James Lu. All rights reserved.
 * ScoreSystem.cs: Saves the score persistently score across levels.
 */

using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour {
    private static ScoreSystem instance;
    private ScoreSystem() { }
    public static ScoreSystem Instance { get { return instance; } }

    public static Color scoreWarningTextColor = new Color(1f, 0.5f, 0.2f, 1f);

    // The textbox to write into
    public Text textField;

    // Score tracking
    [Tooltip("The current score")]
    public int score;

    [Tooltip("Sets the amount of time to wait before increasing the score based on elapsed time")]
    private int scoreInterval = 1;

    void Awake () {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        // Make ourselves always available.
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        InvokeRepeating("AddScoreTime", scoreInterval, scoreInterval);
    }

    // Add the given amount to the current score, and update the score text.
    public int AddScore(int amount)
    {
        score += amount;
        textField.text = "Score: " + score.ToString();
        if (GameState.Instance.gameOverOnZeroScore)
        {
            if (score <= 0)
            {
                // Don't let time flow negative if the level is timed.
                GameState.Instance.LevelEnd(false);
            }
            else if (score <= GameState.Instance.scoreWarningThreshold)
            {   // Warn that the player is running out of time once score gets below a certain amount.
                textField.color = scoreWarningTextColor;
            }
        }
        return score;
    }

    // Update the score based on time
    void AddScoreTime()
    {
        // Skip if there is no level running.
        if (!GameState.Instance.gameEnded) {
            AddScore(-scoreInterval);
        }
    }
}
