using UnityEngine;

public enum AutoMoverValue { positionx, positiony }

public class AutoMover : MonoBehaviour
{
    public AutoMoverValue targetValue;
    public float startValue;
    public float endValue;
    public float animationLength;

    public string GetAnimationAttribute(AutoMoverValue value)
    {
        switch (value)
        {
            case (AutoMoverValue.positionx):
                return "m_LocalPosition.x";
            case (AutoMoverValue.positiony):
                return "m_LocalPosition.y";
        }
        return null;
    }

    void Start()
    {

        //Animation anim = GetComponent<Animation>();
        Animator anim = GetComponent<Animator>();

        AnimatorOverrideController controller = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = controller;

        // First value: start value, second value: start time, third: animation length, fourth: end value
        // https://docs.unity3d.com/ScriptReference/AnimationCurve.Linear.html
        AnimationCurve curve = AnimationCurve.Linear(startValue, 0, animationLength, endValue);
        AnimationClip clip = new AnimationClip();
        //clip.legacy = true;

        string attributeName = GetAnimationAttribute(targetValue);
        //Keyframe keyNew = new Keyframe(5, 10f);
        clip.SetCurve("", typeof(Transform), attributeName, curve);
        //anim.AddClip(clip, "AutoMover");
        controller["movement"] = clip;
        anim.Play("movement");
        //anim.StartPlayback();
    }
}