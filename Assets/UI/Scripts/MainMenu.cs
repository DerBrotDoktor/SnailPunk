using UnityEngine;

public class MainMenu : MonoBehaviour
{
    
    
    public void OnStartButtonClicked()
    {
        SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
        sceneLoader.LoadGame();
    }
    
    public void OnSettingsButtonClicked()
    {
        SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
        sceneLoader.LoadSettingsMenu();
    }

    public void OnCreditsButtonClicked()
    {
        SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
        sceneLoader.LoadCredits();
    }
    
    public void OnQuitButtonClicked()
    {
        #if UNITY_EDITOR
            
            UnityEditor.EditorApplication.isPlaying = false;
        
        #else
            
            Application.Quit();
        
        #endif

    }
}
