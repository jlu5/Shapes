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
   // private Sprite playerSprite = Resources.Load<Sprite>("Player");
    private GameObject playerList;
    private GameObject canvasTemplate;
    private GameObject canvas;

    // Draws on the canvas the current player & a list of players as sprites
    void MakeHUD()
    {
        //Canvas canvas = FindObjectOfType<Canvas>();
        playerOverlay = Resources.Load<GameObject>("PlayerOverlay");
        playerList = Resources.Load<GameObject>("PlayerList");
        canvasTemplate = Resources.Load<GameObject>("HUDCanvas");

        canvas = Instantiate(canvasTemplate);
        // Clone the player list prefab (pre-defined object) and add it to the canvas.
        // Canvases are essentially screens drawn over the UI, which can be used for menus, HUDs, etc.
        //GameObject newPlayerList = Instantiate(playerList);
        //newPlayerList.transform.SetParent(canvas.transform);
        for (int i = 0; i < playerCount; i++)
        {
            // For each player, create a new instance of our player overlay prefab and move it into the Canvas.
           
            GameObject newObj = Instantiate(playerOverlay);
            newObj.name = "Player Overlay for Player " + i;
            //float spriteSize = newObj.GetComponent<SpriteRenderer>().bounds.size.x;
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