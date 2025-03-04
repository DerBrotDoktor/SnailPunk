using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private PlayableDirector fadeIn;
    [SerializeField] private PlayableDirector fadeOut;
    
    private const string MAIN_SCENE = "Main";
    private const string GAME_SCENE = "GameScene";
    private const string UI_SCENE = "InGameUI";
    private const string MAIN_MENU_SCENE = "MainMenu";
    private const string SETTINGS_SCENE = "SettingsMenu";
    private const string CREDITS_SCENE = "Credits";

    private void Awake()
    {
        #if !UNITY_EDITOR

        LoadMainMenu();
        
        #endif
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadGame()
    {
        fadeIn.Play();
        
        UnloadScenes();
        SceneManager.LoadScene(GAME_SCENE, LoadSceneMode.Additive);
        SceneManager.LoadScene(UI_SCENE, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnGameSceneLoaded;
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != GAME_SCENE) return;
        
        SceneManager.sceneLoaded -= OnGameSceneLoaded;
        SceneManager.SetActiveScene(scene);
    }

    public void LoadMainMenu()
    {
        fadeIn.Play();
        
        UnloadScenes();
        SceneManager.LoadScene(MAIN_MENU_SCENE, LoadSceneMode.Additive);
    }

    public void LoadSettingsMenu()
    {
        fadeIn.Play();
        SceneManager.LoadScene(SETTINGS_SCENE, LoadSceneMode.Additive);
    }

    public void UnloadSettingsMenu()
    {
        fadeIn.Play();
        SceneManager.UnloadSceneAsync(SETTINGS_SCENE);
    }

    public void LoadCredits()
    {
        fadeIn.Play();
        UnloadScenes();
        SceneManager.LoadScene(CREDITS_SCENE, LoadSceneMode.Additive);
    }
    
    private void UnloadScenes() {
        
        int c = SceneManager.sceneCount;
        
        for (int i = 0; i < c; i++) {
            
            Scene scene = SceneManager.GetSceneAt (i);
            
            if (scene.name != MAIN_SCENE) {
                SceneManager.UnloadSceneAsync (scene);
            }
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fadeIn.Stop();
        fadeOut.Play();
    }
}
