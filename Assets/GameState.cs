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

    // TODO: make the current player configurable in via level data
    public int currentPlayer = 1;

    // Player/Level state tracking
    public int playerCount;
    public bool gameEnded;
    private Dictionary<int, Player> players = new Dictionary<int, Player>();
    private Dictionary<System.Type, Dictionary<int, Collidable>> collidables = new Dictionary<System.Type, Dictionary<int, Collidable>>();

    // How quickly should the camera pan?
    public float cameraPanTime = 0.1F;
    // How quickly should we zoom the camera?
    public float cameraZoomSpeed = 1F;
    // What is the biggest camera view allowed?
    public float cameraMaxSize = 15.0F;
    // What is the smallest camera view allowed?
    public float cameraMinSize = 5.0F;

    // Resource templates, used by Instantiate()
    private GameObject canvasTemplate;
    private GameObject simpleCanvasTemplate;
    private GameObject stretchedTextLabelTemplate;
    private GameObject fadeToColourTemplate;

    // Access to the current HUDCanvas instance.
    private HUDCanvas canvas;

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

        // Load our relevant resources
        canvasTemplate = Resources.Load<GameObject>("HUDCanvas");
        simpleCanvasTemplate = Resources.Load<GameObject>("SimpleHUDCanvas");
        stretchedTextLabelTemplate = Resources.Load<GameObject>("StretchedTextLabel");
        fadeToColourTemplate = Resources.Load<GameObject>("FadeToColour");

        // Initialize the characters list HUD
        canvas = Instantiate(canvasTemplate).GetComponent<HUDCanvas>();
    }

    // Method called to end the current level.
    public void LevelEnd(bool win=true)
    {
        gameEnded = true;
        GameObject levelEndCanvas = Instantiate(simpleCanvasTemplate);
        GameObject levelEndText = Instantiate(stretchedTextLabelTemplate);
        Text text = levelEndText.GetComponent<Text>();
        text.text = "You win!"; // TODO: implement the win boolean with alternative losing text
        text.fontSize *= 4;  // Make the text bigger

        // Add a fade out image.
        Instantiate(fadeToColourTemplate).transform.SetParent(levelEndCanvas.transform, false);

        // Add the "game over" text, but make sure to keep the right world space position.
        // This can be done by setting the worldPositionStays option (second argument) in
        // setParent to false.
        levelEndText.transform.SetParent(levelEndCanvas.transform, false);
    }

    // Adds a player into the current scene.
    public void AddPlayer(int id, Player player)
    {
        // Register the player into the player list. TODO: make sure
        // the key doesn't already exist.
        players[id] = player;

        // Add the player to the player list canvas.
        canvas.AddPlayer(id, player);
    }

    // Returns the requested player by ID.
    public Player GetPlayer(int id)
    {
        return players[id];
    }

    // Removes a player from the current scene.
    public void RemovePlayer(int id)
    {
        // First, remove our player overlay.
        canvas.RemovePlayer(id);

        // Then, destroy the gameobject and storage related to it.
        Player player = players[id];
        Destroy(player.gameObject);
        players.Remove(id);
    }

    // Registers a collidable with a given ID.
    public void RegisterCollidable(int id, Collidable obj)
    {
        // Internally (to prevent a ton of variables from being used), this stores objects as a
        // dictionary of dictionaries: first by their type, and then by their ID.

        System.Type type = obj.GetType();
        if (!collidables.ContainsKey(type))
        { // Fill in the type key corresponding to the object if it doesn't exist
            collidables[type] = new Dictionary<int, Collidable>();
        }

        collidables[type][id] = obj;
    }

    // Fetches a registered collidable, returning null if it is missing.
    public Collidable GetCollidable<T>(int id)
    {
        try
        {
            return collidables[typeof(T)][id];
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
                string keyName = "Fire" + btnNum;

                try
                {
                    if (Input.GetButtonDown(keyName))
                    {
                        Debug.Log("Current player set to " + btnNum);
                        currentPlayer = btnNum;
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

            // Catch attempts to reload the scene (defaults to Esc key)
            if (Input.GetButtonDown("Reset")) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
