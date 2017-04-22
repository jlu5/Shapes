using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorBlueprint : MonoBehaviour {

    public string objectType;

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
                Editor.Instance.SetClickedObject(gameObject, objectType);
            }
        }
    }

    public void SetAttribute(string name, string value)
    {
        if (Editor.Instance != null && !string.IsNullOrEmpty(value))
        {
            switch (name)
            {
                // Handle position and scale changes.
                case "positionx":
                    transform.position = new Vector3(float.Parse(value), transform.position.y, transform.position.z);
                    break;
                case "positiony":
                    transform.position = new Vector3(transform.position.x, float.Parse(value), transform.position.z);
                    break;
                case "scalex":
                    transform.localScale = new Vector3(float.Parse(value), transform.localScale.y, transform.position.z);
                    break;
                case "scaley":
                    transform.localScale = new Vector3(transform.localScale.x, float.Parse(value), transform.position.z);
                    break;
            }
        }
    }

    public string GetAttribute(string name)
    {
        switch (name)
        {
            case "positionx":
                return transform.position.x.ToString();
            case "positiony":
                return transform.position.y.ToString();
            case "scalex":
                return transform.localScale.x.ToString();
            case "scaley":
                return transform.localScale.y.ToString();
            default:
                return "";
        }
    }
}