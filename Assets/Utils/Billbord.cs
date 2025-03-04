using UnityEngine;

public class Billbord : MonoBehaviour
{
    [SerializeField] private bool freezeX = false;
    [SerializeField] private bool freezeY = false;
    [SerializeField] private bool freezeZ = false;
    
    Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        Vector3 newRotation = mainCamera.transform.eulerAngles;

        if (freezeX) newRotation.x = transform.eulerAngles.x;
        if (freezeY) newRotation.y = transform.eulerAngles.y;
        if (freezeZ) newRotation.z = transform.eulerAngles.z;

        transform.eulerAngles = newRotation;
    }
}
