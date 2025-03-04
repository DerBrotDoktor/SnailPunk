using UnityEngine;
using FMODUnity;

public class CameraZoomToFMOD : MonoBehaviour
{
    public Camera mainCamera;
    public string parameterName = "Camera Zoom"; // Replace with your FMOD global parameter name

    void Start()
    {
        // Automatically find the main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("Main Camera not assigned or found!");
            return;
        }

        // Get the current vertical Field of View (FoV) of the camera
        float fov = mainCamera.fieldOfView;

        // Normalize the FoV to the range your FMOD global parameter expects
        // Assuming your FMOD parameter expects a value between 0 and 1
        float normalizedFov = Mathf.InverseLerp(17.0f, 80.0f, fov); // Adjust these values to your camera's FoV range

        // Set the FMOD global parameter value based on the normalized FoV
        RuntimeManager.StudioSystem.setParameterByName(parameterName, fov);
    }
}
