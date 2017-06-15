/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * GameState.cs: Implements global game state tracking - the core of the game.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// Singleton method adapted from https://msdn.microsoft.com/en-us/library/ff650316.aspx
public sealed class GameState : MonoBehaviour
{
    // The current instance is privately tracked here.
    private static GameState instance;

    // Make the constructor private so other classes can't create an instance of GameState.
    private GameState() { }

    // Implement a read-only Instance attribute which returns the current instance
    public static GameState Instance
    {
        get
        {
            return instance;
        }
    }

    // EDITOR OPTIONS
    [Tooltip("Sets the initial score amount")]
    public int initialScore = 1000;
    [Tooltip("Sets the amount of time to wait before increasing the score based on elapsed time")]
    public int scoreInterval = 1;
    [Tooltip("Sets whether the game should end when score reaches 0.")]
    public bool gameOverOnZeroScore;
    [Tooltip("Sets the time remaining before we should warn the player that they're about to run out of time (if Death On Zero Score is on)")]
    public int scoreWarningThreshold = 15;
    [Tooltip("Sets the player ID for the game to initially start at.")]
    public int initialPlayer = 1;
    [Tooltip("Sets the amount of finishes needed to complete the level")]
    public int finishesNeeded = 1;
    [Tooltip("Sets the amount of coins needed to complete the level")]
    public int coinsNeeded = 0;
    [Tooltip("How quickly should the camera pan?")]
    public float cameraPanTime = 0.1F;
    [Tooltip("How quickly should we zoom the camera?")]
    public float cameraZoomSpeed = 1F;
    [Tooltip("What is the biggest camera view allowed?")]
    public float cameraMaxSize = 15.0F;
    [Tooltip("What is the smallest camera view allowed?")]
    public float cameraMinSize = 5.0F;
    [Tooltip("Sets the game speed.")]
    public float timeScale = 1.0F;
    [Tooltip("Sets the text to display when the player wins the level.")]
    public string winText = "You win!";
    [Tooltip("Sets the text to display when the player loses the level.")]
    public string loseText = "Game Over";
    [Tooltip("Sets whether any player dying (e.g. by falling off the screen) will trigger a game over.")]
    public bool playerDeathFatal = true;

    // GAME STATE VARIABLES
    // Sets the amount of finishes already completed.
    public int finishCount { get; set; }
    // Sets the amount of coins taken so far.
    public int coinCount { get; set; }
    // Tracks the current player
    public int currentPlayer { get; set; }
    // Tracks the amount of players added to the level; this is automatically incremented with every addPlayer call
    public int playerCount { get; set; }
    // Tracks whether the game is over.
    public bool gameEnded { get; set; }
    // Track lists of players and game scripts based on their ID.
    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    private Dictionary<System.Type, Dictionary<int, MonoBehaviour>> gameScripts = new Dictionary<System.Type, Dictionary<int, MonoBehaviour>>();

    // Resource templates, used by Instantiate()
    private GameObject stretchedTextLabelTemplate;
    private GameObject levelEndScreenTemplate;
    public GameObject textLabelTemplate;

    // Access to various game objects
    public PlayerList playerList {get; protected set; }
    public GameObject powerupsPanel {get; protected set; }
    public GameObject coinCountText {get; protected set; }

    // Score tracking
    [Tooltip("The current score")]
    public int score;
    public static Color scoreWarningTextColor = new Color(1f, 0.5f, 0.2f, 1f);
    // Stores the score text display object
    private Text scoreText;

    void Awake()
    {
        // Sets the current instance to the global one. TODO: make this thread safe?
        if (instance == null)
        {
            instance = this;
        }

        // Requirement for UI elements: Create an EventSystem object with a default input module,
        // if one doesn't already exist.
        EventSystem eventSystem;
        StandaloneInputModule sim;
        if (EventSystem.current)
        {
            Debug.Log("GameState: Using existing EventSystem");
            eventSystem = EventSystem.current;
            sim = eventSystem.gameObject.GetComponent<StandaloneInputModule>();
        }
        else
        {
            Debug.Log("GameState: No EventSystem found; creating a new one");
            GameObject eventSystemObject = new GameObject();
            eventSystemObject.name = "ShapesEventSystem";
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
            sim = eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        // Use a separate input key for overlay buttons to not override keys like Enter and Space
        sim.submitButton = "ClickOnly";

        // Set the time scale to what's defined in the level data. This also unpauses the game if it was
        // previously paused.
        Time.timeScale = timeScale;

        // Load our relevant resources and game objects
        stretchedTextLabelTemplate = Resources.Load<GameObject>("StretchedTextLabel");
        levelEndScreenTemplate = Resources.Load<GameObject>("LevelEndScreen");
        textLabelTemplate = Resources.Load<GameObject>("CanvasTextLabel");
        playerList = transform.Find("PlayerListCanvas").GetComponent<PlayerList>();
        scoreText = transform.Find("HUDCanvas/ScoreText").GetComponent<Text>();
        powerupsPanel = transform.Find("HUDCanvas/PowerupsPanel").gameObject;
        coinCountText = transform.Find("HUDCanvas/CoinCountWrapper/CoinCountText").gameObject;
    }

    void Start()
    {
        // Set the initial score.
        AddScore(initialScore);

        // Start the score updater in a loop.
        InvokeRepeating("AddScoreTime", scoreInterval, scoreInterval);

        // Set the initial player defined in the config.
        currentPlayer = initialPlayer;
    }

    // Method called to end the current level.
    public void LevelEnd(bool win=true)
    {
        gameEnded = true;
        // Create a text object showing "You win" or "Game Over"
        GameObject levelEndText = Instantiate(stretchedTextLabelTemplate);
        Text text = levelEndText.GetComponent<Text>();
        text.text = win ? winText : loseText;
        text.fontSize *= 4;  // Make the text bigger

        // Add a fade out image.
        GameObject levelEndScreen = Instantiate(levelEndScreenTemplate);
        levelEndScreen.transform.SetParent(transform.Find("HUDCanvas"), false);

        // Add the "game over" text to the canvas, but make sure to keep the right world space position.
        // This can be done by setting the worldPositionStays option (second argument) in
        // setParent to false.
        levelEndText.transform.SetParent(levelEndScreen.transform, false);

        if (!LevelSelector.Instance)
        {
            // If no level selector instance is present, disable the level selection buttons.
            levelEndScreen.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    // Registers a game script with a given ID.
    public void RegisterGameScript(int id, MonoBehaviour obj)
    {
        // Internally (to prevent a ton of variables from being used), this stores objects as a
        // dictionary of dictionaries: first by their type, and then by their ID.

        System.Type type = obj.GetType();
        if (!gameScripts.ContainsKey(type))
        { // Fill in the type key corresponding to the object if it doesn't exist
            gameScripts[type] = new Dictionary<int, MonoBehaviour>();
        }

        gameScripts[type][id] = obj;
    }

    // Fetches a registered game script, returning null if it is missing.
    public MonoBehaviour GetGameScript<T>(int id)
    {
        try
        {
            return gameScripts[typeof(T)][id];
        }
        catch (KeyNotFoundException)
        {
            return null;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameEnded)
        {
            // Look up every possible FireX key, where X is the player number.
            // This means that Fire1 switches to the 1st player, Fire2 switches to the 2nd player, etc.
            for (int btnNum = 1; btnNum <= playerCount; btnNum++)
            {
                if (!players.ContainsKey(btnNum))
                {
                    // Don't allow switching to a player that has been removed.
                    continue;
                }
                string keyName = "Fire" + btnNum;

                try
                {
                    if (Input.GetButtonDown(keyName))
                    {
                        Debug.Log("Current player set to " + btnNum);
                        currentPlayer = btnNum;
                        Player player = players[currentPlayer];

                        // If we've already tried panning to the player, but it's still invisible, force-update
                        // the position to match the player: this prevents long waits.
                        if (player.triedPanning && !player.visible)
                        {
                            Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y,
                                Camera.main.transform.position.z);
                        }
                        player.triedPanning = true;
                    }
                }
                catch (System.ArgumentException)
                {
                    // This key doesn't exist, so ignore.
                    Debug.LogWarning(string.Format("Tried to use Fire{0} for player {0}, but it wasn't bounded!",
                                                   btnNum));
                }

            }
            // Handle camera zoom: by default this is bound to the scroll wheel
            float scroll = Input.GetAxis("Zoom");
            float cameraSize = Camera.main.orthographicSize;
            float newSize = cameraSize + scroll;
            // Only zoom if the new camera size is between the minimum and maximum sizes.
            if (newSize < cameraMaxSize && newSize > cameraMinSize)
            {
                Camera.main.orthographicSize = newSize;
            }
        }

        // Catch attempts to reload the scene (defaults to Esc key), but only if the level selector interface isn't loaded.
        // (Otherwise, LevelSelector will handle this and pop up a navigation menu)
        if (!LevelSelector.Instance && Input.GetButtonDown("Reset"))
        {
            RestartLevel();
        }
    }

    // Add the given amount to the current score, and update the score text.
    public int AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score.ToString();
        if (gameOverOnZeroScore)
        {
            if (score <= 0)
            {
                // Don't let time flow negative if the level is timed.
                LevelEnd(false);
            }
            else if (score <= scoreWarningThreshold)
            {   // Warn that the player is running out of time once score gets below a certain amount.
                scoreText.color = scoreWarningTextColor;
            }
        }
        return score;
    }
    // Update the score based on time - this is a simple, parameter free wrapper around AddScore()
    void AddScoreTime()
    {
        AddScore(-scoreInterval);
    }

    // Level selector-based methods.
    public void RestartLevel()
    {
        gameEnded = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
