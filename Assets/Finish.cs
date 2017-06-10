/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Finish.cs: Implements a finish object
 */
using UnityEngine;

public class Finish : Collidable {
    public int playersNeeded = 0;
    private GameObject text;
    private GameObject finishWrapper;

	void Start () {
        // The finish object creates a wrapper object around itself so that it can place a text label
        // without scaling it as the finish graphic pulsates.
        finishWrapper = new GameObject();
        finishWrapper.name = "FinishWrapper";

        // Make the finish wrapper inherit the position of the finish object placed in the editor.
        finishWrapper.transform.position = transform.position;

        // Move the finish object into the wrapper.
        transform.SetParent(finishWrapper.transform);

        // Then, we can add the text label
        text = Instantiate(Resources.Load<GameObject>("SimpleTextMesh"));
        text.transform.SetParent(finishWrapper.transform);
        text.name = "FinishLabel";
        // Clear the local position of the text label so that it's centered with respect to the wrapper.
        text.transform.localPosition = Vector3.zero;
        TextUpdate();
    }

    void TextUpdate()
    {
        // Update the finish label to display the count of players needed to end the level.
        text.GetComponent<TextMesh>().text = playersNeeded.ToString();
    }

    // Method called when a player hits the finish
    public override void PlayerHit(Player player)
    {
        playersNeeded--;
        // Remove the player, but after setting its "finished" attribute to True.
        player.finished = true;
        Destroy(player);

        if (playersNeeded < 1)
        {
            GameState.Instance.finishCount++;
            if (GameState.Instance.finishesNeeded > 0 && GameState.Instance.finishCount >= GameState.Instance.finishesNeeded) {
                // End the level now if we've reached the amount of finishes needed.
                GameState.Instance.LevelEnd();
            }
            // The finish has received all players needed, so destroy it.
            Destroy(finishWrapper);
            return;
        }
        TextUpdate();
    }
}
