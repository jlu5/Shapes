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

        for (int i = 0; i < playerCount; i++)
        {
            // For each player, create a new instance of our player overlay prefab and move it into the Canvas.
           
            GameObject newObj = Instantiate(playerOverlay);
            newObj.name = "Player Overlay for Player " + i;
            newObj.transform.SetParent(canvas.transform);
        }

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