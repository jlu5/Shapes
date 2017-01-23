using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Singleton method adapted from https://msdn.microsoft.com/en-us/library/ff650316.aspx
public sealed class GameState : MonoBehaviour
{
    private static GameState instance;

    private GameState() { }

    public static GameState Instance
    {
        get
        {
            return instance;
        }
    }

    // TODO: make player settings configurable in via level data
    public int currentPlayer = 1;
    public int playerCount = 2;
    private Dictionary<int, Player> players;

    private GameObject playerOverlay;
    private GameObject canvasTemplate;
    private GameObject canvas;
    private GameObject HUDTextLabelTemplate;

    // Draws on the canvas the current player & a list of players as sprites
    void MakeHUD()
    {
        playerOverlay = Resources.Load<GameObject>("PlayerOverlay");
        canvasTemplate = Resources.Load<GameObject>("HUDCanvas");
        HUDTextLabelTemplate = Resources.Load<GameObject>("HUDTextLabel");

        canvas = Instantiate(canvasTemplate);
        GameObject playerListLabel = Instantiate(HUDTextLabelTemplate);
        playerListLabel.GetComponent<Text>().text = "Character list: ";
        playerListLabel.transform.SetParent(canvas.transform);

    }

    // Adds a player into the current scene.
    public void addPlayer(int id, Player player)
    {
        // Create a new instance of our player overlay prefab - this uses the
        // same sprite as the player but has no movement attached.
        GameObject newObj = Instantiate(playerOverlay);
        // Set the object name and sprite color
        newObj.name = "Player Overlay for Player " + id;
        newObj.GetComponent<Image>().color = player.getColor();
        // Move the object into the Canvas.
        newObj.transform.SetParent(canvas.transform);

        // Finally, register the player into the player list. TODO: make sure
        // the key doesn't already exist.
        players[id] = player;
    }

    void Awake()
    {
        // TODO: make this thread safe
        instance = this;

        // Keep the game state code alive, even as we load different levels.
        DontDestroyOnLoad(gameObject);
        MakeHUD();
    }

    // Update is called once per frame
    void Update()
    {
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
            }

        }
    }
}