/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Editor.cs: Level editor stub
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

    // References to GameObjects and various templates
    private GameObject editorOverlayTemplate;
    private Sprite editorDummySprite;
    public GameObject displayBracket;
    private GameObject itemsCanvas;
    private GameObject editorCanvas;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        editorOverlayTemplate = Resources.Load<GameObject>("EditorOverlay");
        editorDummySprite = Resources.Load<Sprite>("editordummysprite");

        // The editor scene uses two canvases: ItemsCanvas uses a layout group to sort all displayable items/tools,
        // while EditorCanvas is freeform (used for the scrolling arrows, etc.)
        itemsCanvas = GameObject.Find("ItemsCanvas");
        editorCanvas = GameObject.Find("EditorCanvas");

        // Create a display bracket object representing the currently selected item: EditorOverlay
        // instances will later enable this and move it to the right positions.
        displayBracket = Instantiate(Resources.Load<GameObject>("SelectionBracketOverlay"));
        displayBracket.SetActive(false);
        displayBracket.transform.SetParent(editorCanvas.transform);

        foreach (string item in supportedObjects)
        {
            // For each supported item, initialize the respective prefab into a templates dictionary.
            GameObject template = Resources.Load<GameObject>(item);
            templates[item] = template;

            Debug.Log("Editor: loading resource " + item);

            // Initialize the items list: use our editor overlay prefab for each object,
            // but set its sprite to match the object it represents.
            Sprite objectSprite;
            SpriteRenderer spriteRenderer = template.GetComponent<SpriteRenderer>();
            GameObject overlay = Instantiate(editorOverlayTemplate, itemsCanvas.transform);
            if (spriteRenderer == null)
            {
                // For objects without a sprite renderer (e.g. SimpleTextMesh), fall back to a
                // dummy sprite.
                objectSprite = editorDummySprite;
            } else
            {
                objectSprite = spriteRenderer.sprite;
            }
            overlay.GetComponent<Image>().sprite = objectSprite;

            // Set the editor overlay's resource name, so that it can be clicked.
            overlay.GetComponent<EditorOverlay>().resourceName = item;
        }

        // Now, append sprites for the delete and settings tools. These aren't implemented yet...
        foreach (string spritename in new string[] { "delete", "configure"})
        {
            Sprite sprite = Resources.Load<Sprite>(spritename);
            GameObject overlay = Instantiate(editorOverlayTemplate, itemsCanvas.transform);
            overlay.GetComponent<EditorOverlay>().resourceName = spritename;
            overlay.GetComponent<Image>().sprite = sprite;
        }
    }

    private void Update()
    {
        // On left mouse click, place the currently active object in the world where the mouse is.
        if (Input.GetMouseButtonUp(0) && !string.IsNullOrEmpty(currentObject))
        {

            // UI raycasting is annoying ... Derived from
            // http://answers.unity3d.com/questions/844158/how-do-you-perform-a-graphic-raycast.html
            // Basically this creates a PointerEventData object with the current mouse position,
            // and passes that into EventSystem.current.RaycastAll(). This then tests the given position
            // for any UI object collisions, and dumps the results into the raycastResults list.
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            if (raycastResults.Count == 0)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameObject newObject = Instantiate(templates[currentObject], mousePosition, Quaternion.identity);
                newObject.transform.position = new Vector3(newObject.transform.position.x, newObject.transform.position.y, 0);

                Debug.Log("Editor: adding new object!");
            }


        }
    } 
}
