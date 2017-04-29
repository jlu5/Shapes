using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMaterial : MonoBehaviour {
    public bool isBouncy;
    public bool isIce;
    public bool enable = true;
    private bool movable;
    private Color color;

    // Material definitions
    private PhysicsMaterial2D materialBouncy;
    private PhysicsMaterial2D materialIce;

    // Color definitions. TODO: describe how they're used
    protected Color staticColor;
    protected Color dynamicColor;
    protected Color staticBouncyColor;
    protected Color dynamicBouncyColor;
    protected Color staticIceColor;
    protected Color dynamicIceColor;

    // I personally prefer working with hex colors over rgb values, because they're easier to tweak/replace.
    void Awake() {
        staticColor = Utils.HexColor("#FFFFFF");
        dynamicColor = Utils.HexColor("#FF955C");
        staticBouncyColor = Utils.HexColor("#FFD62D");
        dynamicBouncyColor = Utils.HexColor("#94FFB8");
        staticIceColor = Utils.HexColor("#C9FDFF");
        dynamicIceColor = Utils.HexColor("#9EE3EF");

        materialBouncy = Resources.Load<PhysicsMaterial2D>("materialBouncy");
        materialIce = Resources.Load<PhysicsMaterial2D>("materialIce");
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
        if (!enable)
        {
            return;
        }

        movable = IsDynamic();
        Collider2D collider = GetComponent<Collider2D>();
        // If the bouncy attribute is set or the object already has bouncy attributes, update the color + material.
        if (isBouncy || (collider.sharedMaterial == materialBouncy))
        {
            color = movable ? dynamicBouncyColor : staticBouncyColor;
            collider.sharedMaterial = materialBouncy;
        }
        // Ditto with ice
        else if (isIce || (collider.sharedMaterial == materialIce))
        {
            color = movable ? dynamicIceColor : staticIceColor;
            collider.sharedMaterial = materialIce;
        }
        else
        {
            color = movable ? dynamicColor : staticColor;
        }
        GetComponent<SpriteRenderer>().color = color;
    }

	// Use this for initialization
	void Start () {
        Run();
	}
}
