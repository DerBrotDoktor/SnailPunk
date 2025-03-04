using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    private const string MASTER_BUS_NAME = "bus:/";
    private const string MUSIC_BUS_NAME = "bus:/Music";
    private const string SFX_BUS_NAME = "bus:/SFX";
    private const string VO_BUS_NAME = "bus:/VO";
    
    private FMOD.Studio.Bus masterBus;
    private FMOD.Studio.Bus musicBus;
    private FMOD.Studio.Bus sfxBus;
    private FMOD.Studio.Bus voBus;

    public void AddMasterVolume(float value)
    {
        masterBus.getVolume(out float master);
        masterBus.setVolume(master + value);
    }
    
    public void SetMasterVolume(float value)
    {
        masterBus.setVolume(value);
    }
        
    public void SetMusicVolume(float value)
    {
        musicBus.setVolume(value);
    }
        
    public void SetSFXVolume(float value)
    {
        sfxBus.setVolume(value);
    }

    public void SetVOVolume(float value)
    {
        voBus.setVolume(value);
    }
}
