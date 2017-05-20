/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * PlayerOverlay.cs: Implements clickable player sprites for HUDCanvas instances
 */
using UnityEngine;
using UnityEngine.UI;

public class PlayerOverlay : ClickableOverlay
{
    public int playerID;
    public bool showPlayerID = true;
    private GameObject label;

    protected override void Start()
    {
        base.Start(); // Initialize the base ClickableOverlay class

        if (showPlayerID)
        {
            // Label which player this overlay corresponds to
            label = Instantiate(GameState.Instance.textLabelTemplate);
            Text labeltext = label.GetComponent<Text>();
            labeltext.text = playerID.ToString();

            // Use the same colour as the player object's overlay (for consistency)
            labeltext.color = Player.textLabelColor;

            // Move the label under the player overlay
            label.transform.SetParent(transform);
        }
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
