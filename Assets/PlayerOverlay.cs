using UnityEngine;
using UnityEngine.UI;

public class PlayerOverlay : MonoBehaviour
{
    public int playerID;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (playerID != 0)
        {
            Debug.Log("Clicked player overlay of player ID " + playerID);
            GameState.Instance.currentPlayer = playerID;
        }
        else
        {
            Debug.LogWarning("PlayerOverlay clicked but not bound to a player ID!");
        }
    }
}