using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class GameStateCore {


}
*/

// Singleton method from https://msdn.microsoft.com/en-us/library/ff650316.aspx
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
    public int current_player = 1;
    public int player_count = 2;

    void Awake()
    {
        // TODO: make this thread safe
        instance = this;

        // Keep the game state code alive, even as we load different levels.
        DontDestroyOnLoad(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        for (int btn_num = 1; btn_num <= player_count; btn_num++)
        {
            string keyname = "Fire" + btn_num;

            try
            {
                if (Input.GetButtonDown(keyname))
                {
                    Debug.Log("Current player set to " + btn_num);
                    current_player = btn_num;
                }
            }
            catch (System.ArgumentException)
            {
            }

        }
    }
}