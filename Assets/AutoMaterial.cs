using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMaterial : MonoBehaviour {
    // Configurable options: whether the object is bouncy, icy, or sticky. Only one of these should be enabled at a time..
    public bool isBouncy;
    public bool isIce;
    public bool isSticky;

    // Toggles automatic color settings.
    public bool enableColor = true;

    // Misc state tracking
    private bool movable;
    private Color color;

    // Material definitions
    private PhysicsMaterial2D materialBouncy;
    private PhysicsMaterial2D materialIce;
    private PhysicsMaterial2D materialSticky;

    // Color definitions. 
    protected Color staticColor;
    protected Color dynamicColor;
    protected Color staticBouncyColor;
    protected Color dynamicBouncyColor;
    protected Color staticIceColor;
    protected Color dynamicIceColor;
    protected Color stickyColor;

    // I personally prefer working with hex colors over rgb values, because they're easier to tweak/replace.
    // See Run() below for a rough description of what each color corresponds to.
    void Awake() {
        staticColor = Utils.HexColor("#FFFFFF");
        dynamicColor = Utils.HexColor("#FF955C");
        staticBouncyColor = Utils.HexColor("#FFD62D");
        dynamicBouncyColor = Utils.HexColor("#94FFB8");
        staticIceColor = Utils.HexColor("#C9FDFF");
        dynamicIceColor = Utils.HexColor("#9EE3EF");
        stickyColor = Utils.HexColor("#A18454");

        materialBouncy = Resources.Load<PhysicsMaterial2D>("materialBouncy");
        materialIce = Resources.Load<PhysicsMaterial2D>("materialIce");
        materialSticky = Resources.Load<PhysicsMaterial2D>("materialSticky");
    }

    bool IsDynamic() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            return false;
        }
        else
        {
            return rb.bodyType == RigidbodyType2D.Dynamic;
        }
    }

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
