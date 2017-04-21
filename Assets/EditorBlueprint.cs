using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorBlueprint : MonoBehaviour {

    void OnMouseUpAsButton()
    {
        if (Editor.Instance != null)
        {
            string currentTool = Editor.Instance.currentObject;
            if (currentTool == "delete")
            {
                Debug.Log("Deleting object " + gameObject.name);
                Destroy(gameObject);
            } else if (currentTool == "configure")
            {
                Debug.Log("Editor: currently configuring " + gameObject.name);
                Editor.Instance.currentlyConfiguring = gameObject;
            }
        }
    }

    public void SetAttribute(string name, string value)
    {
        if (Editor.Instance != null)
        {
            switch (name)
            {
                case "transformx":
                    transform.position = new Vector3(float.Parse(value), transform.position.y, transform.position.z);
                    break;
                case "transformy":
                    transform.position = new Vector3(transform.position.x, float.Parse(value), transform.position.z);
                    break;
            }
        }
    }
}