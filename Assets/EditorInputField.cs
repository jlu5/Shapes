using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class EditorInputField : MonoBehaviour {

    public InputField myInput;
    public string targetField;

    void UpdateAttribute(InputField input)
    {
        GameObject target = Editor.Instance.currentlyConfiguring;
        if (target != null) // Don't do anything if we don't have an object selected.
        {
            // Pass the attribute update to the editor blueprint we're currently
            // configuring.
            EditorBlueprint targetScript = target.GetComponent<EditorBlueprint>();
            targetScript.SetAttribute(targetField, input.text);
        }
    }

    void Start()
    {
        myInput = GetComponent<InputField>();
        myInput.onValueChanged.AddListener(delegate { UpdateAttribute(myInput); });
    }
}
