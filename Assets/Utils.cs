using System;
using UnityEngine;
using UnityEngine.UI;

public static class Utils {
    // Returns a normalized sum of two colors.
    public static Color ColorSum(Color first, Color second)
    {
        // Separate the components, add them, and cap them at 1.
        Debug.Log(string.Format("first color values: {0}, {1}, {2}, {3}", first.r, first.g, first.b, first.a));
        Debug.Log(string.Format("second color values: {0}, {1}, {2}, {3}", second.r, second.g, second.b, second.a));
        float r = Math.Min(1, first.r + second.r);
        float g = Math.Min(1, first.g + second.g);
        float b = Math.Min(1, first.b + second.b);
        float a = Math.Min(1, first.a + second.a);
        // Return the new color sum.
        Debug.Log(string.Format("sum: {0}, {1}, {2}, {3}", r, g, b, a));
        return new Color(r, g, b, a);
    }
    
    // Returns a normalized difference of two colors.
    public static Color ColorDifference(Color first, Color second)
    {
        // Separate the components, subtract them, and cap them at 0.
        Debug.Log(string.Format("first color values: {0}, {1}, {2}, {3}", first.r, first.g, first.b, first.a));
        Debug.Log(string.Format("second color values: {0}, {1}, {2}, {3}", second.r, second.g, second.b, second.a));
        float r = Math.Max(0, first.r - second.r);
        float g = Math.Max(0, first.g - second.g);
        float b = Math.Max(0, first.b - second.b);

        // Note: transparency is left alone for now...
        Debug.Log(string.Format("difference: {0}, {1}, {2}, {3}", r, g, b, first.a));
        return new Color(r, g, b, first.a);
    }

    // Returns a hex color given a hex string (or HTML color code).
    public static Color HexColor(string hexstring) {
        Color color;
        // First arg = hex string, second arg = color variable to output to
        ColorUtility.TryParseHtmlString(hexstring, out color);
        return color;
    }

    // Updates the text of a GameObject with either UI.Text or TextMesh.
    public static void SetText(GameObject gameObject, string text)
    {
        Text uiText = null;
        TextMesh meshText = null;

        // Try to look up the UI-based text element.
        try {
            uiText = gameObject.GetComponent<Text>();
            // Ignore if the component is missing.
        } catch (NullReferenceException) {
        } catch (MissingReferenceException) {
        }

        // Try to find the in-world TextMesh object.
        try {
            meshText = gameObject.GetComponent<TextMesh>();
        } catch (NullReferenceException) {
        } catch (MissingReferenceException) {
        }

        if (uiText != null)
        {
            uiText.text = text;
        } else if (meshText != null)
        {
            meshText.text = text;
        }
    }

    // Adds an explosion force centered around GameObject with the given explosion radius and max force.
    // This method returns the amount of objects affected.
    public static int AddExplosionForce2D(GameObject gobject, float explosionRadius, float explosionForce)
    {
        // Basically what we need to do is find a list of objects close to the bomb when it explodes,
        // and propel them away. The closer the object is to the bomb, the more force is applied on it.
        Collider2D[] colliderResults = Physics2D.OverlapCircleAll(gobject.transform.position, explosionRadius);
        foreach (Collider2D collider in colliderResults)
        {
            if (collider.attachedRigidbody)
            {
                // Calculate the difference between the target object and the bomb's positions.
                Vector2 differenceVector = collider.transform.position - gobject.transform.position;
                float distance = differenceVector.magnitude;

                // Calculate a force amount inversely proportional to the distance betwen the two objects.
                float force = Mathf.Min(1 / distance, distance) * explosionForce;

                // Fetch the angle between the bomb and the target object.
                float explosionAngle = Mathf.Atan2(differenceVector.y, differenceVector.x);

                // Create a new vector based off this angle.
                Vector2 targetVector = new Vector2(force * Mathf.Cos(explosionAngle), force * Mathf.Sin(explosionAngle));

                collider.attachedRigidbody.AddForce(targetVector, ForceMode2D.Impulse);

                Debug.Log("Difference from object " + collider.gameObject.name + " to bomb: " + differenceVector.ToString());
                Debug.Log("Explosion angle is " + explosionAngle + " radians with object " + collider.gameObject.name);
                Debug.Log("cos(" + explosionAngle.ToString() + ") is " + Mathf.Cos(explosionAngle).ToString());
                Debug.Log("Using force " + force.ToString() + " on object " + collider.gameObject.name);
                Debug.Log("Adding force of magnitude " + targetVector.ToString() + " to object " + collider.gameObject.name);
            }
        }
        return colliderResults.Length;
    }
}
