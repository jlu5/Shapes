/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * BindDisplay.cs: Displays a line between two arbitrary game objects
 */
using UnityEngine;

public class BindDisplay : MonoBehaviour {
    public GameObject object1;
    public GameObject object2;

    // Quick access to the line renderer component
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    } 

    // Update is called once per frame
    void Update () {
        // The bind display needs two objects two draw between; nothing is displayed otherwise.
        if (object1 == null || object2 == null)
        {
            return;
        }
        // Make lines draw over characters, so that they're actually visible.
        lr.sortingLayerName = "PlayerForeground";
        lr.sortingOrder = -1;

        // Draw a line between this object and the other one (set the line renderer's positions).
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[2] { object1.transform.position, object2.transform.position });

    }
}
