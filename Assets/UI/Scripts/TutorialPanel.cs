using System;
using System.Collections.Generic;
using TMPro;
using Tutorial;
using UnityEngine;

namespace UI
{
    public class TutorialPanel : MonoBehaviour
    {
        
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text taskText;
        
        [SerializeField] private TMP_Text flavorText;
        [SerializeField] private GameObject flavorTextContainer;
        
        [SerializeField] private GameObject finishContainer;
        
        private const string STRIKE_START = "<s>";
        private const string STRIKE_END = "</s>";

        private void Awake()
        {
            flavorTextContainer.SetActive(false);
            tutorialPanel.SetActive(false);
            finishContainer.SetActive(false);
        }

        public void SetFlavorText(string text)
        {
            tutorialPanel.SetActive(false);
            flavorText.text = text;
            flavorTextContainer.SetActive(true);
        }

        public void OnCloseFlavorText()
        {
            flavorTextContainer.SetActive(false);
            FindObjectOfType<TutorialController>().FinishStep();
        }
        
        public void UpdatePanel(Step step, List<Task> tasks, List<bool> taskFinished)
        {
            titleText.text = step.Name;
            
            taskText.text = "";
            foreach (var task in tasks)
            {
                int i = tasks.IndexOf(task);

                if(string.IsNullOrWhiteSpace(task.Text)) continue;

                if (taskFinished[i])
                {
                    taskText.text += STRIKE_START + task.Text + STRIKE_END + "\n";
                }
                else
                {
                    taskText.text += task.Text + "\n";
                }
                
            }

            tutorialPanel.SetActive(true);
        }

        public void OnTutorialFinished()
        {
            tutorialPanel.SetActive(false);
            flavorTextContainer.SetActive(false);
            finishContainer.SetActive(true);

            foreach (Transform trans in transform.parent)
            {
                trans.gameObject.SetActive(trans == transform);
            }
            
            Time.timeScale = 0f;
        }

        public void OnContinueButtonClicked()
        {
            FindObjectOfType<SceneLoader>().LoadMainMenu();
        }

        public void OnStayButtonClicked()
        {
            finishContainer.SetActive(false);
            
            foreach (Transform trans in transform.parent)
            {
                trans.gameObject.SetActive(true);
            }
            
            Time.timeScale = 1f;
        }
    }
}