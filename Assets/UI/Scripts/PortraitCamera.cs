using UnityEngine;

public class PortraitCamera : MonoBehaviour
{

    [SerializeField] private Vector3 cameraOffset;

    public void SetTarget(GameObject target)
    {
        if (target == null)
        {
            transform.parent = null;
            transform.position = Vector3.zero;
            return;
        }
        
        transform.parent = target.transform;
        transform.localPosition = cameraOffset;
        
        Vector3 direction = target.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction, target.transform.up);
    }
}
