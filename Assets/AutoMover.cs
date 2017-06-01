using UnityEngine;
using System;

public enum AutoMoverValue { positionX, positionY }

public class AutoMover : MonoBehaviour
{
    // Defines the target X and Y position values for the object.
    public float endX;
    public float endY;

    // Determines how long the animation should last in one direction.
    public float animationLength = 1f;

    void Start()
    {
        // Retrieve the animator and create an override for it.
        Animator anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError(string.Format("AutoMover instance on object {0} is missing Animator component! Please add one in the editor, set it to use \"AutoMover\" as controller, and reload.",
                                         name));
            return;
        }

        AnimatorOverrideController controller = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = controller;

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
        clip.SetCurve("", typeof(Transform), "localPosition.x", Xcurve);
        clip.SetCurve("", typeof(Transform), "localPosition.y", Ycurve);

        // Replace the dummy animation placeholders in the animation controller, and play.
        controller["AutoMoverPlaceholder"] = clip;
        anim.Play("AutoMover");
    }
}