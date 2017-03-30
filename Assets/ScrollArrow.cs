using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollArrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public string direction = "up";
    private bool moving;
    public float cameraPanTime = 0.05F;

    public void OnPointerEnter(PointerEventData data)
    {
        moving = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        moving = false;
    }

    void Update()
    {
        if (moving)
        {
            /*Vector3 offset;
            if (direction == "up")
            {
                offset = Vector3.up;
            } else if (direction == "down")
            {
                offset = Vector3.down;
            }*/
            Vector3 offset = Vector3.zero;
            // Use reflection to get the direction vector; it's either this or a bunch of if statements...
            offset = (Vector3) offset.GetType().GetProperty(direction).GetValue(direction, null);

            Vector3 target = Camera.main.transform.position + offset;
            // Initialize a zero velocity variable, and pass it as a reference to SmoothDamp (i.e. "ref velocity")
            Vector3 velocity = Vector3.zero;
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position,
                target, ref velocity, cameraPanTime);
        }
    }
}
