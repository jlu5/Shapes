using UnityEngine;

public class Finish : Collidable {
    private GameObject simpleTextMesh;
    public int playersNeeded = 0;
    private GameObject text;

	void Start () {
        // The finish object creates a wrapper around itself so it can place the text label
        // without scaling it as the finish graphic pulsates.
        GameObject finishWrapper = new GameObject();
        finishWrapper.name = "FinishWrapper";
        transform.SetParent(finishWrapper.transform);

        // Then, we can add the text label
        simpleTextMesh = Resources.Load<GameObject>("SimpleTextMesh");
        text = Instantiate(simpleTextMesh);
        text.transform.SetParent(finishWrapper.transform);
        text.name = "FinishLabel";
        textUpdate();
    }

    void textUpdate()
    {
        // Update the finish label to display the new count of players needed.
        text.GetComponent<TextMesh>().text = System.Convert.ToString(playersNeeded);
    }

    // Method called when a player hits the finish
    public override void PlayerHit(Player player)
    {
        playersNeeded -= 1;
        // Remove the player
        Destroy(player.gameObject);

        if (playersNeeded < 1)
        {
            // The finish has received all players needed, so destroy it.
            Destroy(gameObject);
            return;
        }
        textUpdate();
    }
}
