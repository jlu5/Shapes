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
        Text uiText = gameObject.GetComponent<Text>();
        if (uiText != null)
        {
            uiText.text = text;
        }
    }
}
