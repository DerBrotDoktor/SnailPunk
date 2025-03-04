using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace UI
{
    public class BuildModeInfo : MonoBehaviour
    {
        [SerializeField] private BuildModeInfoStruct[] buildModeInfos = new BuildModeInfoStruct[4];
        private Dictionary<BuildMode, BuildModeInfoStruct> buildModeInfoDictionary = new Dictionary<BuildMode, BuildModeInfoStruct>();
        private BuildMode currentMode = BuildMode.None;
        
        private UnityEngine.Rendering.Universal.Vignette vignette;

        [SerializeField] private GameObject textContainer;
        [SerializeField] private TMP_Text text;

        private void Awake()
        {
            foreach (var buildModeInfo in buildModeInfos)
            {
                if (!buildModeInfoDictionary.TryAdd(buildModeInfo.mode, buildModeInfo))
                {
                    throw new ArgumentException($"Duplicate build mode {buildModeInfo.mode}");
                }
            }

            VolumeProfile volumeProfile = GetComponent<Volume>().profile;
            if(!volumeProfile.TryGet(out vignette)) throw new ArgumentException("Vignette not found on BuildModeInfo");
            
            vignette.active = false;
            textContainer.gameObject.SetActive(false);
        }

        public void SetBuildModeInfo(BuildMode mode)
        {
            currentMode = mode;

            vignette.color.Override(buildModeInfoDictionary[mode].color);
            
            text.text = buildModeInfoDictionary[mode].text;
            
            vignette.active = true;
            textContainer.gameObject.SetActive(true);
        }
    
        public void ClearBuildingModeInfo(BuildMode mode)
        {
            if (currentMode != mode) return;
        
            vignette.active = false;
            textContainer.gameObject.SetActive(false);
        }
    }

    [Serializable]
    public struct BuildModeInfoStruct
    {
        public BuildMode mode;
        public Color color;
        public string text;
    }
}
