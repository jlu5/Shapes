using System;
using UnityEngine;

public class Finish : Collidable {
    private GameObject simpleTextMesh;
    public int playersNeeded = 0;
    private GameObject text;
    private GameObject finishWrapper;

	void Start () {
        // The finish object creates a wrapper around itself so it can place the text label
        // without scaling it as the finish graphic pulsates.
        finishWrapper = new GameObject();
        finishWrapper.name = "FinishWrapper";

        // Move the position attributes of the finish object into the wrapper, so that this
        // applies to the text label as well.
        finishWrapper.transform.position = transform.position;

        // Move the finish object into the wrapper.
        transform.SetParent(finishWrapper.transform);

        // Then, we can add the text label
        simpleTextMesh = Resources.Load<GameObject>("SimpleTextMesh");
        text = Instantiate(simpleTextMesh);
        text.transform.SetParent(finishWrapper.transform);
        text.name = "FinishLabel";
        text.transform.localPosition = Vector3.zero;
        TextUpdate();
    }

    void TextUpdate()
    {
        // Update the finish label to display the new count of players needed.
        text.GetComponent<TextMesh>().text = System.Convert.ToString(playersNeeded);
    }

    // Method called when a player hits the finish
    public override void PlayerHit(Player player)
    {
        playersNeeded -= 1;
        // Remove the player
        GameState.Instance.RemovePlayer(player.playerID);

        if (playersNeeded < 1)
        {
            GameState.Instance.LevelEnd();
            // The finish has received all players needed, so destroy it.
            Destroy(finishWrapper);
            return;
        }
        TextUpdate();
    }
}
