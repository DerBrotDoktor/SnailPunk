using System;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Sound
{
    public class BuildingInfoPanelSoundEmitter : MonoBehaviour
    {
        [SerializeField] private EventReference defaultSound;
        [SerializeField] private List<BuildingInfoPanelSoundStruct> sounds;
        private EventInstance eventInstance;
        
        public void Play(string name)
        {
            EventReference sound = defaultSound;

            foreach (var soundStruct in sounds.Where(soundStruct => name == soundStruct.buildingData.name))
            {
                sound = soundStruct.sound;
                break;
            }
            
            eventInstance = RuntimeManager.CreateInstance(sound);
            eventInstance.start();
            eventInstance.release();
        }
    }

    [Serializable]
    public struct BuildingInfoPanelSoundStruct
    {
        public EventReference sound;
        public BuildingData buildingData;
    }
}
