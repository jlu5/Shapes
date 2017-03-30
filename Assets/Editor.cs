/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Editor.cs: Level editor stub
 */
using UnityEngine;
using System.Collections.Generic;

public class Editor : MonoBehaviour {
    // Defines which objects are items that can be added by the editor.
    private string[] supportedObjects = new string[] {"PlayerObject", "CircleWall", "DoorKeyObject", "DoorObject",
                                                      "FinishObject", "SimpleTextMesh", "TriangleWall", "Wall"};
    private Dictionary<string, GameObject> templates = new Dictionary<string, GameObject>();

    void Awake()
    {
        foreach (string item in supportedObjects)
        {
            // For each supported item, initialize the respective prefab into a templates dictionary.
            GameObject template = Resources.Load<GameObject>(item);
            templates[item] = template;

            // TODO: add an item to the ItemsCanvas with the same sprite as the item.
            //Sprite sprite = template.GetComponent<SpriteRenderer>().sprite;

            Debug.Log("Editor: loading resource " + item);
        }
    }
}
