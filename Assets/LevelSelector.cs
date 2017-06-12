using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Structures to help deserialize level data from JSON
[System.Serializable]
public struct LevelData
{
    public string name;
    public string path;
    public string thumbnail;
}

[System.Serializable]
public class LevelDataCollection
{
    public LevelData[] levels;
}

public class LevelSelector : MonoBehaviour {
    // Make LevelSelector a singleton class - see GameState.cs for more details
    private static LevelSelector instance;
    private LevelSelector() { }
    public static LevelSelector Instance
    {
        get
        {
            return instance;
        }
    }

    // Set the default level pack.
    public static string defaultLevelPack = "default.levelpack";

    // Track level packs and their levels internally
    public string levelsPath { get; set; }
    private List<string> levelPacks = new List<string>();
    public LevelDataCollection levelData;

    // Track the ID of the last level (this allows the "Next level" button to work
    public int lastLevel { get; set; }

    // References to various game elements
    public GameObject gamePausedText { get; protected set; }
    private GameObject levelSelectButtonTemplate;
    private GameObject levelSelectPanel;
    private GameObject levelPackSelector;

    public void Quit() {
        Application.Quit();
    }

    public void SwitchCanvas(int canvasNum) {
        Debug.Log(string.Format("Running SwitchCanvas({0})", canvasNum));

        // First, hide/disable all canvas objects.
        foreach (GameObject canvas in GameObject.FindGameObjectsWithTag("ToggleableCanvas"))
        {
            canvas.SetActive(false);
        }

        // Then, enable the one requested.
        if (canvasNum >= 0) {
            transform.GetChild(canvasNum).gameObject.SetActive(true);
            // Reset the time scale if it was previously changed.
            Time.timeScale = 1.0F;
        }
    }

    // Loads a level pack and initializes the level choosing screen
    void InitLevelPack(string packPath)
    {
        packPath = levelsPath + packPath;
        string jsonData = File.ReadAllText(packPath);
        Debug.Log("Got raw JSON data for " + packPath + " :" + jsonData.Replace("\n", " ").Replace("\r", ""));
        levelData = JsonUtility.FromJson<LevelDataCollection>(jsonData);

        // First, clear all levels from the level list.
        foreach (Transform child in levelSelectPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int idx = 0; idx < levelData.levels.Length; idx++)
        {
            LevelData level = levelData.levels[idx];
            GameObject newbtn = Instantiate(levelSelectButtonTemplate);

            // Set up the button corresponding to that level.
            LevelSelectButton lsb = newbtn.GetComponentInChildren<LevelSelectButton>();
            lsb.levelName = level.path;
            lsb.levelNum = idx;

            // TODO: replace the button BG with a thumbnail

            // Set the text related to the level entry: the level name and the level count.
            Utils.SetText(newbtn.transform.GetChild(0).GetChild(0).gameObject, (idx+1).ToString());
            Utils.SetText(newbtn.transform.GetChild(1).gameObject, level.name);

            newbtn.transform.SetParent(levelSelectPanel.transform);
        }
    }

    public void PlayNextLevel() {
        int targetLevel = lastLevel + 1;
        // Find the button representing the next level, and emulate a click 
        // (this is lazy but it means we don't have to track a list of levels manually)
        if (targetLevel < levelSelectPanel.transform.childCount) {
            levelSelectPanel.transform.GetChild(targetLevel).gameObject.GetComponentInChildren<LevelSelectButton>().OnClick();
        }
        else
        {
            Debug.Log("No level left, returning to level selector screen");
            SceneManager.LoadScene("LevelSelect");
        }
    }

    void Awake () {
        if (instance == null)
        {
            // Register our level selector instance if none exists
            instance = this;
        } else
        {
            // A level selector instance already exists, so remove the duplicate.
            Destroy(gameObject);
            Instance.SwitchCanvas(1);  // Go to the level list.
            return;
        }

        // Make the level selector core always available.
        DontDestroyOnLoad(gameObject);

        // Initialize object references...
        levelSelectPanel = GameObject.Find("LevelSelectMainPanel");
        levelPackSelector = GameObject.Find("LevelPackSelector");
        levelSelectButtonTemplate = Resources.Load<GameObject>("LevelSelectButton");
        gamePausedText = GameObject.Find("GamePausedText");

        // Load our scene asset bundle.
        AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/LevelAssetBundles/levels");

        // Disable the level pack selector while we load its available options.
        levelPackSelector.SetActive(false);

        // Load all available level packs and add them into the dropdown menu.
        levelsPath = Application.streamingAssetsPath + "/Levels/";
        
        // Add the default level pack first.
        levelPacks.Add(defaultLevelPack);
        foreach (string fullpath in Directory.GetFiles(levelsPath, "*.levelpack", SearchOption.AllDirectories)) {
            // Note: use the filename and not the long, complete path.
            string path = Path.GetFileName(fullpath);
            if (path != defaultLevelPack) // Don't add the default level pack twice.
            {
                levelPacks.Add(path);
            }
        }
        Dropdown dropdown = levelPackSelector.GetComponent<Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(levelPacks);

        // Add a listener to change the levels list when a new level pack is selected.
        dropdown.onValueChanged.AddListener(
            delegate (int packIndex)
            {
                InitLevelPack(levelPacks[packIndex]);
            }
        );
        Utils.SetText(levelPackSelector.transform.GetChild(0).gameObject, "Select Level Pack");
        levelPackSelector.SetActive(true);

        // Initialize the default level pack.
        InitLevelPack(defaultLevelPack);

        // Enable the welcome canvas and disable the rest. Note: we can't leave the object pre-disabled in the scene because GameObject.Find() doesn't work on inactive objects.
        SwitchCanvas(0);
    }

    void Update()
    {
        // Handle the Reset action (defaults to the Esc key): pause the game and show a menu of navigation buttons.
        if (Input.GetButtonDown("Reset") && GameState.Instance)
        {
            if (!transform.GetChild(2).gameObject.activeInHierarchy)
            {
                // Enable the canvas and pause the game if not already.
                SwitchCanvas(2);
                Time.timeScale = 0;
            }
            else
            {
                // Otherwise, do the reverse.
                SwitchCanvas(-1);
                Time.timeScale = GameState.Instance.timeScale;
            }
        }
    }
}
