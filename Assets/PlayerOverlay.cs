/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * PlayerOverlay.cs: Implements clickable player sprites for HUDCanvas instances
 */
using UnityEngine;
using UnityEngine.UI;

public class PlayerOverlay : ClickableOverlay
{
    public int playerID;
    private GameObject HUDTextLabelTemplate;
    private GameObject label;

    protected override void Start()
    {
        base.Start(); // Initialize the base ClickableOverlay class

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
    public override void OnClick()
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
