using System;
using Tutorial;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

public class Game : MonoBehaviour
{

    public static Action TimeScaleChanged;
    private static int _timeScale = 0;
    public static int TimeScale
    {
        get
        {
            return _timeScale;
        }
        set
        {
            if (_timeScale != value)
            {
                _timeScale = value;
                TimeScaleChanged?.Invoke();
            }
        }
    }
    
    
    [SerializeField] private bool skipIntroCutscene = false;
    
    [Space(15)]
    [SerializeField] private PlayableDirector introCutsceneDirector;
    
    private GameObject canvas;

    private void Start()
    {
        Time.timeScale = 1f;
        Debug.Log("[Game] Start");
        canvas = GameObject.Find("InGameUICanvas");
        
#if UNITY_EDITOR
        if (skipIntroCutscene)
        {
            introCutsceneDirector.time = introCutsceneDirector.duration - 0.1f;
        }
#endif
        StartIntroCutscene();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TimeScale = TimeScale == 0 ? 1 : 0;
        }
    }

    private void StartIntroCutscene()
    {
        print("[Game] Intro cutscene started.");

        canvas.SetActive(false);
        introCutsceneDirector.Play();
    }
    
    public void OnIntroCutsceneFinished()
    {
        print("[Game] Intro cutscene finished. " + canvas);
        
        TimeScale = 1;
        
        canvas.SetActive(true);
        
        FindObjectOfType<TutorialController>().StartTutorial();
    }
}
