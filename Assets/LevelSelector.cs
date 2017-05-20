using System.IO;
using System.Collections.Generic;
using UnityEngine;
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

    public string levelsPath { get; set; }
    public string defaultLevelPack = "demo.levelpack";
    public LevelDataCollection levelData;
    public List<string> levelPacks { get; set; }
    public int lastLevel { get; set; }

    private GameObject levelSelectButtonTemplate;
    private GameObject welcomeCanvas;
    private GameObject levelSelectCanvas;
    private GameObject levelSelectPanel;

    public void Quit() {
        Application.Quit();
    }

    public void SwitchCanvas(int canvasNum) {
        // First, hide/disable all canvas objects.
        Debug.Log(string.Format("Running SwitchCanvas({0})", canvasNum));

        foreach (GameObject canvas in GameObject.FindGameObjectsWithTag("ToggleableCanvas"))
        {
            canvas.SetActive(false);
        }

        GameObject target = null;

        switch (canvasNum) {
            case 0: 
                target = welcomeCanvas;
                break;
            case 1:
                target = levelSelectCanvas;
                break;
            default:
                break;
        }
        if (target != null) {
            target.SetActive(true);
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
            LevelSelectButton lsb = newbtn.GetComponent<LevelSelectButton>();
            lsb.levelName = level.path;
            lsb.levelNum = idx;

            // TODO: replace the button BG with a thumbnail

            // Set the text related to the level entry: the level name and the level count.
            Utils.SetText(newbtn.transform.GetChild(0).gameObject, level.name);
            Utils.SetText(newbtn.transform.GetChild(1).gameObject, (idx+1).ToString());

            newbtn.transform.SetParent(levelSelectPanel.transform);
        }
    }

    public void PlayNextLevel() {
        int targetLevel = lastLevel + 1;
        if (targetLevel < levelSelectPanel.transform.childCount) {
            levelSelectPanel.transform.GetChild(targetLevel).gameObject.GetComponent<LevelSelectButton>().OnClick();
        } else
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
        welcomeCanvas = GameObject.Find("WelcomeCanvas");
        levelSelectCanvas = GameObject.Find("LevelSelectCanvas");
        levelSelectPanel = GameObject.Find("LevelSelectMainPanel");

        // Load our scene asset bundle.
        AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/LevelAssetBundles/levels");

        levelsPath = Application.streamingAssetsPath + "/Levels/";
        // Initialize our level pack list.
        levelPacks = new List<string>();

        levelSelectButtonTemplate = Resources.Load<GameObject>("LevelSelectButton");
        // Load all available level packs.
        foreach (string path in Directory.GetFiles(levelsPath, "*.levelpack", SearchOption.AllDirectories))
        {
            Debug.Log("Found level pack " + path);
            levelPacks.Add(path);
        }

        InitLevelPack(defaultLevelPack);

        // Enable the welcome canvas and disable the rest. Note: we can't leave the object pre-disabled in the scene because GameObject.Find() doesn't work on inactive objects.
        SwitchCanvas(0);
    }
}
