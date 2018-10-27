/* Shapes Game (c) 2018 James Lu. All rights reserved.
 * ScoreSystem.cs: Saves the score persistently score across levels.
 */

using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour {
    private static ScoreSystem instance;
    private ScoreSystem() { }
    public static ScoreSystem Instance { get { return instance; } }

    // Tracks whether this level was accessed sequentially from the last one. If this is false
    // when a level loads, it should reset the score instead of incrementing it.
    public bool sequentialPlaying { get; set; }

    public static Color scoreWarningTextColor = new Color(1f, 0.5f, 0.2f, 1f);

    // The textbox to write into
    public Text textField;

    // Score tracking: This is only public s.t. you can edit it in Unity Editor; AddScore
    // and ResetScore should be used instead.
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

    public int ResetScore(int amount=0)
    {
        score = amount;
        textField.text = "Score: " + score.ToString();
        return score;
    }

    // Update the score based on time
    private void AddScoreTime()
    {
        // Skip if there is no level running.
        if (!GameState.Instance.gameEnded) {
            AddScore(-scoreInterval);
        }
    }
}
