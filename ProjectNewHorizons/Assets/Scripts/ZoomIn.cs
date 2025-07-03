using UnityEngine;

public class ZoomIn : MonoBehaviour
{
    public Camera cam;
    private float zoomSpeed = 0.1f;

    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Positions in the current frame
            Vector2 touch0Pos = touch0.position;
            Vector2 touch1Pos = touch1.position;

            // Positions in the previous frame
            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0Pos - touch1Pos).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            cam.fieldOfView -= difference * zoomSpeed;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 40f, 60f);
        }
    }
}