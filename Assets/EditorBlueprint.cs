using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorBlueprint : MonoBehaviour {

    void OnMouseUpAsButton()
    {
        string currentTool = Editor.Instance.currentObject;
        if (currentTool == "delete")
        {
            Debug.Log("Deleting object " + gameObject.name);
            Destroy(gameObject);
        }
    }
}