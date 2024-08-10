using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 10f; // Speed of zooming in and out
    public float panSpeed = 20f; // Speed of panning the camera
    public float minFieldOfView = 20f; // Minimum field of view (zoom in limit)
    public float maxFieldOfView = 90f; // Maximum field of view (zoom out limit)

    void Update()
    {
        HandleZoom();
        HandlePan();
    }

    void HandleZoom()
    {
        if (Input.GetKey(KeyCode.UpArrow)) // Zoom in
        {
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - zoomSpeed * Time.deltaTime, minFieldOfView, maxFieldOfView);
        }
        if (Input.GetKey(KeyCode.DownArrow)) // Zoom out
        {
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + zoomSpeed * Time.deltaTime, minFieldOfView, maxFieldOfView);
        }
    }

    void HandlePan()
    {
        Vector3 position = transform.position;

        if (Input.GetKey(KeyCode.W)) // Move forward
        {
            position.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S)) // Move backward
        {
            position.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A)) // Move left
        {
            position.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D)) // Move right
        {
            position.x += panSpeed * Time.deltaTime;
        }

        transform.position = position;
    }
}
