/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * AutoMaterial.cs: Automatically colours and sets material of walls and objects.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMaterial : MonoBehaviour {
    [Tooltip("Sets whether this object is icy.")]
    public bool isBouncy;
    [Tooltip("Sets whether this object is bouncy.")]
    public bool isIce;
    [Tooltip("Sets whether this object is sticky.")]
    public bool isSticky;

    [Tooltip("Sets whether automatic colour setting based on AutoMaterial attributes should be used on this object.")]
    public bool enableColor = true;

    // Misc state tracking
    private bool movable;
    private Color color;

    // Material definitions
    private PhysicsMaterial2D materialBouncy;
    private PhysicsMaterial2D materialIce;
    private PhysicsMaterial2D materialSticky;

    // Color definitions. 
    public Color staticColor { get; set; }
    public Color dynamicColor { get; set; }
    public Color staticBouncyColor { get; set; }
    public Color dynamicBouncyColor { get; set; }
    public Color staticIceColor { get; set; }
    public Color dynamicIceColor { get; set; }
    public Color stickyColor { get; set; }
    public Color deadlyColorDifference { get; set; }

    void Awake() {
        // Load color definitions. Note: Utils.HexColor must be called after class initialization,
        // so these can't be statically defined.
        staticColor = Utils.HexColor("#FFFFFF");
        dynamicColor = Utils.HexColor("#FF955C");
        staticBouncyColor = Utils.HexColor("#FFD62D");
        dynamicBouncyColor = Utils.HexColor("#94FFB8");
        staticIceColor = Utils.HexColor("#C9FDFF");
        dynamicIceColor = Utils.HexColor("#9EE3EF");
        stickyColor = Utils.HexColor("#A18454");
        deadlyColorDifference = Utils.HexColor("#059797");

        // Load our material resources.
        materialBouncy = Resources.Load<PhysicsMaterial2D>("materialBouncy");
        materialIce = Resources.Load<PhysicsMaterial2D>("materialIce");
        materialSticky = Resources.Load<PhysicsMaterial2D>("materialSticky");
    }

    // Returns whether the object is dynamic (ie affected by forces)
    bool IsDynamic() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            return false;  // No rigid body automatically implies false
        }
        else
        {
            return rb.bodyType == RigidbodyType2D.Dynamic;
        }
    }

    // Runs the AutoMaterial engine
    public void Run() {
        movable = IsDynamic();
        Collider2D collider = GetComponent<Collider2D>();

        // If the bouncy attribute is set or the object already has bouncy attributes, update the color + material.
        // Bouncy and static = yellow, bouncy and movable = green
        if (isBouncy || (collider.sharedMaterial == materialBouncy))
        {
            color = movable ? dynamicBouncyColor : staticBouncyColor;
            collider.sharedMaterial = materialBouncy;
        }

        // Ditto with ice; both static and dynamic objects are light blue
        else if (isIce || (collider.sharedMaterial == materialIce))
        {
            color = movable ? dynamicIceColor : staticIceColor;
            collider.sharedMaterial = materialIce;

        // Sticky walls are a muddy shade of brown.
        } else if (isSticky || (collider.sharedMaterial == materialSticky)) {
            color = stickyColor;
            collider.sharedMaterial = materialSticky;

        } else
        { // For regular objects, use the generic white color for static walls and orange for moving objects.
            color = movable ? dynamicColor : staticColor;
        }

        if (gameObject.GetComponent<KillOnTouch>())
        {
            // Make the object red if it's marked as deadly.
            color = Utils.ColorDifference(color, deadlyColorDifference);
        }

        // Automatic colour updating is optional!
        if (enableColor)
        {
            GetComponent<SpriteRenderer>().color = color;
        }
    }

	// Use this for initialization
	void Start () {
        Run();
	}
}
