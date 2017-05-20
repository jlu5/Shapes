/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * PlayerList.cs: Implements a canvas displaying a list of player characters
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerList : MonoBehaviour {
    private PlayerOverlay playerOverlay;
    private Dictionary<int, PlayerOverlay> overlays = new Dictionary<int, PlayerOverlay>();
    private GameObject playerListLabel;

    // Initialization: fill in resources and create the player list label
    void Awake () {
        playerOverlay = Resources.Load<PlayerOverlay>("PlayerOverlay");

        playerListLabel = Instantiate(GameState.Instance.textLabelTemplate);
        playerListLabel.GetComponent<Text>().text = "Select character: ";
        playerListLabel.transform.SetParent(transform);
	}

    // Adds a player to the canvas.
	public void AddPlayer(int id, Player player)
	{
		// Create a new instance of our player overlay prefab - this uses the
        // same sprite as the player but has no movement attached.
        PlayerOverlay newObj = Instantiate(playerOverlay);

        // Set the object name and sprite color
        newObj.name = "Player Overlay for Player " + id;
        newObj.GetComponent<Image>().color = player.getColor();

        // Move the object into the Canvas.
        newObj.transform.SetParent(transform);

        // Bind the new object to the player ID.
        newObj.playerID = id;
        overlays[id] = newObj;

        // Sort the player overlays: for each child object with a PlayerOverlay script, set its
        // index in the player list equal to the player ID - 1.
        foreach (Transform child in transform)
        {
            PlayerOverlay overlay = child.gameObject.GetComponent<PlayerOverlay>();
            if (overlay != null)
            {
                Debug.Log(string.Format("PlayerList: sorting PlayerOverlay {0} to position {0}", overlay.playerID));
                child.SetSiblingIndex(overlay.playerID-1);
            }
        }
        playerListLabel.transform.SetSiblingIndex(0);
    }

    // Removes a player from the canvas.
    public void RemovePlayer(int id)
    {
        // Destroy the overlay object and its reference in our overlays tracker.
		Destroy(overlays[id].gameObject);
		overlays.Remove(id);
    }
}
