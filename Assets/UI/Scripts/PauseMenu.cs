using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {

        [SerializeField] private GameObject container;
        [SerializeField] private GameObject menuContainer;
        [SerializeField] private GameObject warningContainer;

        private void Awake()
        {
            Close();
        }

        private void Open()
        {
            Time.timeScale = 0f;

            foreach (Transform trans in transform.parent)
            {
                if(trans == transform) continue;
                trans.gameObject.SetActive(false);
            }
            
            container.SetActive(true);
            warningContainer.SetActive(false);
            menuContainer.SetActive(true);
        }
        
        private void Close()
        {
            foreach (Transform trans in transform.parent)
            {
                trans.gameObject.SetActive(true);
            }
            
            container.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (container.activeSelf)
                {
                    Time.timeScale = 1f;
                    Close();
                }
                else
                {
                    Open();
                }
            }
        }

        public void OnResumeButtonClicked()
        {
            Time.timeScale = 1f;
            Close();
        }
        
        public void OnMainMenuButtonClicked()
        {
            menuContainer.SetActive(false);
            warningContainer.SetActive(true);
        }

        public void OnContinueButtonClicked()
        {
            SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
            sceneLoader.LoadMainMenu();
        }

        public void OnCancelButtonClicked()
        {
            warningContainer.SetActive(false);
            menuContainer.SetActive(true);
        }
        
        public void OnSettingsButtonClicked()
        {
            SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
            sceneLoader.LoadSettingsMenu();
        }
    }
}
