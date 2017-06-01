using UnityEngine;
using System;

public enum AutoMoverValue { positionX, positionY }

public class AutoMover : MonoBehaviour
{

    // Defines the start and end position values for the target.
    //public float startX;
    public float endX;
    //public float startY;
    public float endY;

    // Determines how long the animation should last in one direction.
    public float animationLength;

    /*
    // Fetch Unity's internal animation variable given the friendly value.
    public string GetAnimationAttribute(AutoMoverValue value)
    {
        switch (value)
        {
            case (AutoMoverValue.positionx):
                return "localPosition.x";
            case (AutoMoverValue.positiony):
                return "localPosition.y";
        }
        return null;
    }
    */

    // Fetch the starting animation value (e.g. the X or Y position) given the value name.
    public float GetStartValue(AutoMoverValue value)
    {
        switch (value)
        {
            case (AutoMoverValue.positionX):
                return transform.position.x;
            case (AutoMoverValue.positionY):
                return transform.position.y;
        }
        throw new ArgumentException("Unknown value " + value.ToString());
    }

    void Start()
    {
        // Retrieve the animator and create an override for it.
        Animator anim = GetComponent<Animator>();
        AnimatorOverrideController controller = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = controller;

        // Fetch the starting X and Y values based off the object's initial position.
        float startX = GetStartValue(AutoMoverValue.positionX);
        float startY = GetStartValue(AutoMoverValue.positionY);

        // Create X and Y coordinate animation curves.
        // First argument: start time, second argument: start value, third: animation length, fourth: end value
        // https://docs.unity3d.com/ScriptReference/AnimationCurve.Linear.html
        AnimationCurve Xcurve = AnimationCurve.Linear(0, startX, animationLength, endX);
        AnimationCurve Ycurve = AnimationCurve.Linear(0, startY, animationLength, endY);
        AnimationClip clip = new AnimationClip();

        /* Use the animation curve created above to animate the target property.
         * 
         * https://docs.unity3d.com/ScriptReference/AnimationClip.SetCurve.html
         * First argument: the game object name to apply to (blank means the current object),
         * Second: the type of component to animate; in this case, it is the object's Transform
         * Third: the Unity internal attribute name (hardcoded)
         * Fourth: the animation curve
         */
        clip.SetCurve("", typeof(Transform), "localPosition.x", Xcurve);
        clip.SetCurve("", typeof(Transform), "localPosition.y", Ycurve);

        // Replace the dummy animation placeholders in the animation controller, and play.
        controller["AutoMoverPlaceholder"] = clip;
        anim.Play("AutoMover");
    }
}