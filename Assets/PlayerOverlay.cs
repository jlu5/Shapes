using UnityEngine;
using UnityEngine.UI;

public class PlayerOverlay : MonoBehaviour
{
    public int playerID;
    private GameObject HUDTextLabelTemplate;
    private GameObject label;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        // Label which player this overlay corresponds to
		HUDTextLabelTemplate = Resources.Load<GameObject>("HUDTextLabel");
        label = Instantiate(HUDTextLabelTemplate);
        Text labeltext = label.GetComponent<Text>();
        labeltext.text = playerID.ToString();

        // Use the same colour as the player object's overlay (for consistency)
        labeltext.color = GameState.Instance.GetPlayer(playerID).textLabelColor;

        // Move the label under the player overlay
        label.transform.SetParent(transform);

    }

    // When clicked, set the current player to this one.
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
