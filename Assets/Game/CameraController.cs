using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float zoomSpeed = 2.0f;
    [SerializeField] private float minZoom = 5.0f;
    [SerializeField] private float maxZoom = 20.0f;
    [SerializeField] private float minTiltAngle = 45.0f;
    [SerializeField] private float maxTiltAngle = 60.0f;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float targetStopDistance = 2.0f;
    [SerializeField] private int startZoom = 50;

    // Camera boundaries
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    public Camera myCamera;

    private float currentZoom = 10.0f;
    private Transform targetTransform;

    private Vector3 rotationCenter;
    private Plane groundPlane;

    private void Awake()
    {
        groundPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        currentZoom = startZoom;
    }

    private void Update()
    {
        CalculateRotationCenter();

        if (targetTransform == null)
        {
            HandleMovement(Time.deltaTime);
        }
        else
        {
            HandleTargetMovement(Time.deltaTime);
        }

        HandleZoom();
        HandleTilt();
        HandleRotation();
        
        ClampCameraPosition();
    }

    private void CalculateRotationCenter()
    {
        Ray ray = myCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (groundPlane.Raycast(ray, out float distance))
        {
            rotationCenter = ray.GetPoint(distance);
        }
    }

    private void HandleMovement(float deltaTime)
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(moveHorizontal, 0.0f, moveVertical);
        Vector3 relativeDirection = transform.TransformDirection(direction);
        relativeDirection.y = 0;

        transform.Translate(relativeDirection * (moveSpeed * deltaTime), Space.World);
    }

    private void HandleTargetMovement(float deltaTime)
    {
        Vector3 direction = new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z - targetStopDistance) - transform.position;
        Vector3 relativeDirection = transform.TransformDirection(direction);
        relativeDirection.y = 0;

        transform.Translate(relativeDirection * (moveSpeed * deltaTime * 2), Space.World);

        float zDistance = Vector3.Distance(transform.position, new Vector3(transform.position.x, transform.position.y, targetTransform.position.z));
        float xDistance = Vector3.Distance(transform.position, new Vector3(targetTransform.position.x, transform.position.y, transform.position.z));

        currentZoom = minZoom;
        myCamera.fieldOfView = minZoom;
        transform.rotation = Quaternion.Euler(minTiltAngle, 0, 0);

        if ((zDistance <= targetStopDistance + (moveSpeed * deltaTime * 2)) && (zDistance > targetStopDistance - (moveSpeed * deltaTime * 2)) && (xDistance <= (moveSpeed * deltaTime * 2)))
        {
            targetTransform = null;
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        myCamera.fieldOfView = currentZoom;
    }

    private void HandleTilt()
    {
        float tiltAngle = Mathf.Lerp(maxTiltAngle, minTiltAngle, (currentZoom - minZoom) / (maxZoom - minZoom));
        transform.rotation = Quaternion.Euler(tiltAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    private void HandleRotation()
    {
        if(Time.timeScale == 0f) return;
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.RotateAround(rotationCenter, Vector3.up, mouseX * lookSpeed);
        }
    }
    
    private void ClampCameraPosition()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.z = Mathf.Clamp(position.z, minBounds.y, maxBounds.y);
        transform.position = position;
    }

    public void LookAtTarget(Transform target)
    {
        targetTransform = target;
    }
}
