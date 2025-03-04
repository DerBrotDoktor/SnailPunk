using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingsMenu : MonoBehaviour
    {
        
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider voSlider;
        [SerializeField] private TMP_Dropdown windowModeDropdown;
        
        private const int WINDOW_MODE_WINDOW = 0;
        private const int WINDOW_MODE_FULLSCREEN = 1;
        private const int WINDOW_MODE_BORDERLESS = 2;
        
        private const string MASTER_BUS_NAME = "bus:/";
        private const string MUSIC_BUS_NAME = "bus:/Music";
        private const string SFX_BUS_NAME = "bus:/SFX";
        private const string VO_BUS_NAME = "bus:/VO";
    
        private FMOD.Studio.Bus masterBus;
        private FMOD.Studio.Bus musicBus;
        private FMOD.Studio.Bus sfxBus;
        private FMOD.Studio.Bus voBus;
        
        private void Start()
        {
            masterBus = FMODUnity.RuntimeManager.GetBus(MASTER_BUS_NAME);
            musicBus = FMODUnity.RuntimeManager.GetBus(MUSIC_BUS_NAME);
            sfxBus = FMODUnity.RuntimeManager.GetBus(SFX_BUS_NAME);
            voBus = FMODUnity.RuntimeManager.GetBus(VO_BUS_NAME);
            
            masterBus.getVolume(out var master);
            masterSlider.value = master;
            
            musicBus.getVolume(out var music);
            musicSlider.value = music;
            
            sfxBus.getVolume(out var sfx);
            sfxSlider.value = sfx;
            
            voBus.getVolume(out var vo);
            voSlider.value = vo;

            FullScreenMode mode = Screen.fullScreenMode;
            switch (Screen.fullScreenMode)
            {
                case FullScreenMode.Windowed:
                    windowModeDropdown.value = WINDOW_MODE_WINDOW;
                    break;
                case FullScreenMode.MaximizedWindow:
                    windowModeDropdown.value = WINDOW_MODE_FULLSCREEN;
                    break;
                case FullScreenMode.FullScreenWindow:
                    windowModeDropdown.value = WINDOW_MODE_BORDERLESS;
                    break;
            }
        }
        
        public void OnBackButtonClicked()
        {
            FindObjectOfType<SceneLoader>().UnloadSettingsMenu();
        }
        
        public void OnMasterSliderChanged(Single value)
        {
            masterBus.setVolume(value);
        }
        
        public void OnMusicSliderChanged(Single value)
        {
            musicBus.setVolume(value);
        }
        
        public void OnSFXSliderChanged(Single value)
        {
            sfxBus.setVolume(value);
        }

        public void OnVOSliderChanged(Single value)
        {
            voBus.setVolume(value);
        }
        
        public void OnWindowModeChanged(Int32 value)
        {
            switch (value)
            {
                case WINDOW_MODE_WINDOW:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
                case WINDOW_MODE_FULLSCREEN:
                    Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                    break;
                case WINDOW_MODE_BORDERLESS:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
            }
        }
    }
}
