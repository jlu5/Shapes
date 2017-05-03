using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public string defaultLevelPack = "Levels/demo.levelpack";
    public LevelDataCollection levelData;
    public List<string> levelPacks { get; set; }

    private GameObject levelSelectButtonTemplate;

    // Loads a level pack and initializes the level choosing screen
    void InitLevelPack(string packPath)
    {
        string jsonData = File.ReadAllText(packPath);
        Debug.Log("Got raw JSON data for " + packPath + " :" + jsonData.Replace("\n", " ").Replace("\r", ""));
        levelData = JsonUtility.FromJson<LevelDataCollection>(jsonData);

        // First, clear all levels from the level list.
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        string basePath;
        string thumbnailPath;
        for (int idx = 0; idx < levelData.levels.Length; idx++)
        {
            LevelData level = levelData.levels[idx];
            GameObject newbtn = Instantiate(levelSelectButtonTemplate);

            // Get the base path, and the paths of the scene file and thumbnail.
            basePath = Path.GetDirectoryName(packPath);
            newbtn.GetComponent<LevelSelectButton>().path = Path.Combine(basePath, level.path);
            // TODO: replace the button BG with a thumbnail
            thumbnailPath = Path.Combine(basePath, level.thumbnail);

            // Set the text related to the level entry: the level name and the level count.
            Utils.SetText(newbtn.transform.GetChild(0).gameObject, level.name);
            Utils.SetText(newbtn.transform.GetChild(1).gameObject, (idx+1).ToString());

            //newbtn.GetComponent<Image>();
            newbtn.transform.SetParent(transform);
        }
    }

	void Awake () {
        // Initialize our level pack list.
        levelPacks = new List<string>();

        levelSelectButtonTemplate = Resources.Load<GameObject>("LevelSelectButton");
        // Load all available level packs.
        foreach (string path in Directory.GetFiles("Levels/", "*.levelpack", SearchOption.AllDirectories))
        {
            Debug.Log("Found level pack " + path);
            levelPacks.Add(path);
        }

        InitLevelPack(defaultLevelPack);
    }
}
