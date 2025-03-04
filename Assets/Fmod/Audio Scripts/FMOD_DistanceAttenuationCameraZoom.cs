using UnityEngine;
using Sound;
using FMODUnity;

public class FMOD_DistanceAttenuationCameraZoom : MonoBehaviour
{
    public enum EmitterType
    {
        CustomSoundEmitter,
        FMODEventEmitter
    }

    [Header("Emitter Type")]
    public EmitterType emitterType;

    [Header("Custom Sound Emitter")]
    [SerializeField] private SoundEmitter soundEmitter;

    [Header("FMOD Event Emitter")]
    [SerializeField] private StudioEventEmitter fmodEventEmitter;

    private Camera mainCamera;
    private float referenceFOV = 60f;  // Reference FOV for scaling

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }
    }

    void Update()
    {
        if (mainCamera != null)
        {
            AdjustEventParameter();
        }
    }

    void AdjustEventParameter()
    {
        float currentFOV = mainCamera.fieldOfView;
        float scalingFactor = referenceFOV / currentFOV;

        Debug.Log($"Current FOV: {currentFOV}, Scaling Factor: {scalingFactor}");

        switch (emitterType)
        {
            case EmitterType.CustomSoundEmitter:
                if (soundEmitter != null)
                {
                    soundEmitter.SetParameterByName("DistanceZoom", currentFOV);
                }
                else
                {
                    Debug.LogWarning("SoundEmitter is not assigned!");
                }
                break;

            case EmitterType.FMODEventEmitter:
                if (fmodEventEmitter != null)
                {
                    fmodEventEmitter.SetParameter("DistanceZoom", currentFOV);
                }
                else
                {
                    Debug.LogWarning("FMODEventEmitter is not assigned!");
                }
                break;
        }
    }
}
