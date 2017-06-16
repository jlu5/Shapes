/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * AutoMover.cs: Animate game objects such as coins and walls.
 */

using UnityEngine;
using System;

public class AutoMover : MonoBehaviour
{
    [Tooltip("Defines the target X position value for the object.")]
    public float endX;
    [Tooltip("Defines the target Y position value for the object.")]
    public float endY;
    [Tooltip("Sets the wrap mode for the AutoMover animation. See https://docs.unity3d.com/ScriptReference/WrapMode.html for a list of possible definitions.")]
    public WrapMode wrapmode = WrapMode.PingPong;

    [Tooltip("Determines how long the animation should last in one direction.")]
    public float animationLength = 1f;

    [Tooltip("Sets the door ID (for levers, etc.)")]
    public int ID;

    // Access to the animator component
    public Animation anim { get; set; }

    void Awake()
    {
        // Retrieve the animation component, and create a new one if none exists. This uses Unity's legacy
        // animation component because unfortunately, the new Mecanim/Animator system cannot be scripted
        // on runtime: https://forum.unity3d.com/threads/animationclip-setcurve-doesnt-work-with-mecanim-animations-at-runtime.396547/
        anim = GetComponent<Animation>();
        if (anim == null)
        {
            anim = gameObject.AddComponent<Animation>();
        }
        anim.animatePhysics = true; // Always use the physically accurate animation mode

        /* Create X and Y coordinate animation curves. With AutoMover, the animation begins at the object's
         * initial position.
         * 
         * https://docs.unity3d.com/ScriptReference/AnimationCurve.EaseInOut.html
         * First argument: start time, second argument: start value, third: animation length, fourth: end value
         */
        AnimationCurve Xcurve = AnimationCurve.EaseInOut(0, transform.position.x, animationLength, endX);
        AnimationCurve Ycurve = AnimationCurve.EaseInOut(0, transform.position.y, animationLength, endY);

        /* Use the animation curves created above to animate the target properties.
         * 
         * https://docs.unity3d.com/ScriptReference/AnimationClip.SetCurve.html
         * First argument: the game object name to apply to (blank means the current object),
         * Second: the type of component to animate; in this case, it is the object's Transform
         * Third: the Unity internal attribute name (hardcoded)
         * Fourth: the animation curve
         */
        AnimationClip clip = new AnimationClip();
        clip.legacy = true;
        clip.SetCurve("", typeof(Transform), "localPosition.x", Xcurve);
        clip.SetCurve("", typeof(Transform), "localPosition.y", Ycurve);
        clip.wrapMode = wrapmode;

        // Add the animation clip to the animator and play.
        anim.AddClip(clip, "AutoMover");
        anim.Play("AutoMover");
    }

    void Start()
    {
        if (ID != 0)
        {
            GameState.Instance.RegisterGameScript(ID, this);
        }
    }
}