using UnityEngine;
using FMODUnity;

public class FMODStopOnSceneUnload : MonoBehaviour
{
    void OnDisable()
    {
        StopAllFMODEvents();
    }

    void StopAllFMODEvents()
    {
        // Retrieve the FMOD system instance
        var fmodSystem = RuntimeManager.CoreSystem;

        // Get the list of all currently playing channels
        fmodSystem.getChannelsPlaying(out int channels);

        // Iterate over all channels and stop them
        for (int i = 0; i < channels; i++)
        {
            fmodSystem.getChannel(i, out FMOD.Channel channel);
            channel.stop();
        }

        Debug.Log("All FMOD events have been stopped on scene unload.");
    }
}