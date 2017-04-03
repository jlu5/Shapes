/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Editor.cs: Level editor stub
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public sealed class Editor : MonoBehaviour
{
    // Make Editor a singleton class - see GameState.cs for more details
    private static Editor instance;
    private Editor() { }
    public static Editor Instance
    {
        get
        {
            return instance;
        }
    }

    // Defines which objects are items that can be added by the editor.
    private string[] supportedObjects = new string[] {"PlayerObject", "CircleWall", "DoorKeyObject", "DoorObject",
                                                      "FinishObject", "SimpleTextMesh", "TriangleWall", "Wall"};
    private Dictionary<string, GameObject> templates = new Dictionary<string, GameObject>();

    // Store the currently active object
    public string currentObject;

    private GameObject editorOverlayTemplate;
    private Sprite editorDummySprite;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        editorOverlayTemplate = Resources.Load<GameObject>("EditorOverlay");
        editorDummySprite = Resources.Load<Sprite>("editordummysprite");
        GameObject itemsCanvas = GameObject.Find("ItemsCanvas");

        foreach (string item in supportedObjects)
        {
            // For each supported item, initialize the respective prefab into a templates dictionary.
            GameObject template = Resources.Load<GameObject>(item);
            templates[item] = template;

            Debug.Log("Editor: loading resource " + item);

            /* XXX: needs error handling to work
            Sprite objectSprite;
            SpriteRenderer spriteRenderer = template.GetComponent<SpriteRenderer>();
            GameObject overlay = Instantiate(editorOverlayTemplate, itemsCanvas.transform);
            if (spriteRenderer == null)
            {
                objectSprite = editorDummySprite;
            } else
            {
                objectSprite = spriteRenderer.sprite;
            }
            overlay.GetComponent<Image>().sprite = template.GetComponent<SpriteRenderer>().sprite;
            */
        }
    }

    private void Update()
    {
        // On left mouse click, place the currently active object in the world where the mouse is.
        if (Input.GetMouseButtonUp(0) && !string.IsNullOrEmpty(currentObject))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject newObject = Instantiate(templates[currentObject], mousePosition, Quaternion.identity);
        }
    } 
}
