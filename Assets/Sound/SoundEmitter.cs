using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Sound
{
    public class SoundEmitter : MonoBehaviour
    {
        [SerializeField] private EventReference sound;
        private EventInstance eventInstance;
        public bool use3DAttributes = true;

        private void OnEnable()
        {
            eventInstance = RuntimeManager.CreateInstance(sound);
            if (use3DAttributes)
            {
                Set3DAttributes();
            }

        }

        private void OnDisable()
        {
            eventInstance.release();
        }

        private void Set3DAttributes()
        {
            if (eventInstance.isValid())
            {
                // Set 3D attributes using the position of this GameObject
                eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            }
        }


        public void Play()
        {
            // Set 3D attributes if enabled
            if (use3DAttributes)
            {
                Set3DAttributes();
            }

            eventInstance.start();
        }

        public void PlayOneShot()
        {

            RuntimeManager.PlayOneShot(sound, transform.position);
        }

        public void SetParameterByName(string parameterName, float value)
        {
            if (eventInstance.isValid())
            {
                eventInstance.setParameterByName(parameterName, value);
            }
        }
        public void Stop()
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            
        }
    }
}
