using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ObjectAndProperty
{
    public object root;
    public MemberInfo memberinfo;
    public ObjectAndProperty (object root, MemberInfo memberinfo)
    {

    }
}

public class EditorInputField : MonoBehaviour {

    public InputField myInput;
    public string targetField;

    /*
    private string[] specialProperties = new string[] { "x", "y", "z" };
    */


    /* Return the info field of the given nested value using reflection
     * Based off documentation at https://msdn.microsoft.com/en-us/library/system.reflection.memberinfo(v=vs.110).aspx
     * and http://stackoverflow.com/questions/12680341
     */
    /*
   ObjectAndProperty GetNestedValue(object root, string objectName = "")
   {
       MemberInfo prop = null;
       foreach (string propName in targetField.Split('.'))
       {
           // C# has two main types of instance variables: properties and fields. They have subtle
           // differences that I won't go into detail here, but we need to be able both types when
           // recursing to get our final value.
           // First, test the target variable as a property.
           prop = root.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
           if (prop == null)
           {
               // If that fails, test the target variable as a field.
               prop = root.GetType().GetField(propName, BindingFlags.Public | BindingFlags.Instance);

               if (prop == null)
               {
                   Debug.Log("Failed to access prop " + propName + " of object " + objectName);
                   break;
               }
           }

           // Get the value of the target variable, and repeat the loop. The code to do so varies
           // based on the type of variable we work with, which is why this switch exists.
           switch (prop.MemberType)
           {
               case MemberTypes.Property:
                   root = ((PropertyInfo)prop).GetValue(root, null);
                   break;
               case MemberTypes.Field:
                   root = ((FieldInfo)prop).GetValue(root);
                   break;
           }


           objectName += ".";
           objectName += propName;
       }
       return new ObjectAndProperty(root, prop);
   }

   void UpdateAttribute(InputField input)
   {
       GameObject root = Editor.Instance.currentlyConfiguring;
       //ShapesGameEntity targetScript = target.GetComponent<ShapesGameEntity>();
       //object target = real_target;

       // Get the property that we want, splitting nested ones at a "."
       ObjectAndProperty target = GetNestedValue(root, root.name);
       MemberInfo targetMemberInfo = target.memberinfo;
       if (targetMemberInfo != null)
       {
           // Value setting similarly has varying arguments based on the variable type.
           switch (targetMemberInfo.MemberType)
           {
               case MemberTypes.Property:
                   PropertyInfo targetPropertyInfo = (PropertyInfo)targetMemberInfo;
                   targetPropertyInfo.SetValue(target.root, input.text, null);
                   break;
               case MemberTypes.Field:
                   FieldInfo targetFieldInfo = (FieldInfo)targetMemberInfo;
                   targetFieldInfo.SetValue(target.root, input.text);
                   break;
           }
       }
   }*/

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
