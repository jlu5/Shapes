using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCanvas : MonoBehaviour {
	private GameObject HUDTextLabelTemplate;
    private PlayerOverlay playerOverlay;
    private Dictionary<int, PlayerOverlay> overlays = new Dictionary<int, PlayerOverlay>();

	// Use this for initialization
	void Awake () {
		HUDTextLabelTemplate = Resources.Load<GameObject>("HUDTextLabel");
        playerOverlay = Resources.Load<PlayerOverlay>("PlayerOverlay");

        GameObject playerListLabel = Instantiate(HUDTextLabelTemplate);
        playerListLabel.GetComponent<Text>().text = "Select character: ";
        playerListLabel.transform.SetParent(transform);
	}

	public void addPlayer(int id, Player player)
	{
		// Create a new instance of our player overlay prefab - this uses the
        // same sprite as the player but has no movement attached.
        PlayerOverlay newObj = Instantiate(playerOverlay);

        // Set the object name and sprite color
        newObj.name = "Player Overlay for Player " + id;
        newObj.GetComponent<Image>().color = player.getColor();

        // Move the object into the Canvas.
        newObj.transform.SetParent(transform);

        // Fix the position of the sprite within the character list
        // The index happens to equal the ID, since element 0 is the
        // "Character list" description text.
        newObj.transform.SetSiblingIndex(id);

        // Bind the new object to the player ID.
        newObj.playerID = id;
        overlays[id] = newObj;
    }

    public void removePlayer(int id)
    {
		Destroy(overlays[id].gameObject);
		overlays.Remove(id);
    }
}
