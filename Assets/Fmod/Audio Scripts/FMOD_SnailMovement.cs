using UnityEngine;
using Sound; 

public class FMODSnailMovement : MonoBehaviour
{
    public SoundEmitter idleEmitter;  // Reference to the SoundEmitter for the idle sound
    public SoundEmitter moveEmitter;  // Reference to the SoundEmitter for the movement sound

    private Vector3 previousPosition;
    private bool isMoving = false;

    void Start()
    {
        // Ensure the emitters are set to use 3D attributes
        idleEmitter.use3DAttributes = true;
        moveEmitter.use3DAttributes = true;

        // Play the idle sound at the start
        idleEmitter.Play();

        // Record the initial position of the object
        previousPosition = transform.position;
    }

    void Update()
    {
        // Calculate the object's movement
        Vector3 currentPosition = transform.position;
        bool currentlyMoving = currentPosition != previousPosition;

        // Trigger the movement sound if the object starts moving
        if (currentlyMoving && !isMoving)
        {
            idleEmitter.Stop();
            moveEmitter.Play();
            isMoving = true;
        }
        // Trigger the idle sound if the object stops moving
        else if (!currentlyMoving && isMoving)
        {
            moveEmitter.Stop();
            idleEmitter.Play();
            isMoving = false;
        }

        // Update the previous position for the next frame
        previousPosition = currentPosition;
    }
}
