﻿using UnityEngine;
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
        AnimatorOverrideController controller = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = controller;

        // Fetch the starting X and Y values from the object's initial position.
        float startX = transform.position.x;
        float startY = transform.position.y;

        /* Create X and Y coordinate animation curves. With AutoMover, the animation begins at the object's
         * initial position.
         * 
         * https://docs.unity3d.com/ScriptReference/AnimationCurve.Linear.html
         * First argument: start time, second argument: start value, third: animation length, fourth: end value
         */
        AnimationCurve Xcurve = AnimationCurve.Linear(0, transform.position.x, animationLength, endX);
        AnimationCurve Ycurve = AnimationCurve.Linear(0, transform.position.y, animationLength, endY);

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