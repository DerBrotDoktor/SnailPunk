using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Sound;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Tutorial
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private TutorialData tutorialData;

        private List<Step> steps = new List<Step>();
        private List<Task> currentTasks = new List<Task>();
        private List<bool> taskFinished = new List<bool>();

        private int stepIndex = -1;
        private int finishedTasks = 0;

        private TutorialPanel tutorialPanel;
        private List<InputAction> inputActions = new List<InputAction>();
        private Dictionary<int, float> inputTimes = new Dictionary<int, float>();

        [Header("Sound")]
        [SerializeField] private SoundEmitter taskSoundEmitter;
        [SerializeField] private SoundEmitter stepSoundEmitter;
        
        private EventInstance voiceOverInstance;

        private void Awake()
        {
            if (tutorialData == null)
            {
                throw new ArgumentException("TutorialData is not assigned!");
            }

            steps.AddRange(tutorialData.Steps);

            if (steps.Count == 0)
            {
                throw new ArgumentException("No tutorial steps found in the assigned TutorialData.");
            }
        }
        
        private void OnEnable()
        {
            TutorialEventEmitter.TutorialEventEmitted += OnTutorialEventEmitted;
        }

        private void OnDisable()
        {
            TutorialEventEmitter.TutorialEventEmitted -= OnTutorialEventEmitted;
        }
        
        public void StartTutorial()
        {
            tutorialPanel = FindObjectOfType<TutorialPanel>();

            if (tutorialPanel == null)
            {
                Debug.LogError("No TutorialPanel found in the scene.");
                return;
            }
            
            NextStep();
        }
        
        private float lastRealtime;
        private void Update()
        {
            if(tutorialPanel == null) return;
            
            float deltaTime = Time.realtimeSinceStartup - lastRealtime;
            lastRealtime = Time.realtimeSinceStartup;
            
            if (stepIndex >= steps.Count || steps[stepIndex].IsFlavorText) return;
            
            if (finishedTasks >= currentTasks.Count)
            {
                FinishStep();
            }

            for (int i = 0; i < inputActions.Count; i++)
            {
                var action = inputActions[i];

                if (!action.inProgress || inputTimes[i] < 0) continue;

                inputTimes[i] -= deltaTime;

                if (inputTimes[i] <= 0)
                {
                    for (int j = 0; j < currentTasks.Count; j++)
                    {
                        var task = currentTasks[j];

                        if (task.TaskType == TaskType.Input && task.InputAction == action)
                        {
                            taskFinished[j] = true;
                            finishedTasks++;
                            Debug.Log($"Task {j} completed: {task.TaskType}");
                            tutorialPanel.UpdatePanel(steps[stepIndex], currentTasks, taskFinished);
                            
                            taskSoundEmitter.Play();
                            
                            break;
                        }
                    }
                }
            }

            for (var i  = 0; i < currentTasks.Count; i++)
            {
                Task task = currentTasks[i];
                if (task.TaskType == TaskType.ResourceAmount)
                {
                    if(GlobalInventory.Instance.GetResource(task.ResourceType) >= task.ResourceAmount)
                    {
                        if(taskFinished[i]) continue;
                        
                        taskFinished[i] = true;
                        finishedTasks++;
                        Debug.Log($"Task {taskFinished.Count - 1} completed: {task.TaskType}");
                        tutorialPanel.UpdatePanel(steps[stepIndex], currentTasks, taskFinished);
                        
                        taskSoundEmitter.Play();

                    }
                }
            }
        }

        public void FinishStep()
        {
            Debug.Log($"Tutorial: Step {stepIndex} finished.");

            taskSoundEmitter.Stop();
            stepSoundEmitter.Play();
            
            finishedTasks = 0;
            NextStep();
        }

        private void NextStep()
        {
            DisableInputs();

            stepIndex++;
            if (stepIndex >= steps.Count)
            {
                Debug.Log("Tutorial completed!");
                tutorialPanel.OnTutorialFinished();
                return;
            }

            Step nextStep = steps[stepIndex];

            currentTasks.Clear();
            currentTasks.AddRange(nextStep.Tasks);
            taskFinished = new List<bool>(new bool[currentTasks.Count]);

            inputActions.Clear();
            inputTimes.Clear();
            
            voiceOverInstance.stop(STOP_MODE.IMMEDIATE);
            voiceOverInstance.release();
            
            if(!nextStep.VoiceOver.IsNull)
            {
                voiceOverInstance = RuntimeManager.CreateInstance(nextStep.VoiceOver);
                voiceOverInstance.start();
            }
            
            if (nextStep.IsFlavorText)
            {
                tutorialPanel.SetFlavorText(nextStep.FlavorText);
                return;
            }

            for (int i = 0; i < currentTasks.Count; i++)
            {
                var task = currentTasks[i];

                switch (task.TaskType)
                {
                    case TaskType.Input:
                    {
                        inputActions.Add(task.InputAction);
                        inputTimes.Add(i, task.PressTime);
                        break;
                    }
                    case TaskType.ResourceAmount:
                    case TaskType.Event:
                        break;
                    case TaskType.None:
                    default:
                        Debug.LogError($"Unhandled task type: {task.TaskType}");
                        break;
                }
            }

            Game.TimeScale = nextStep.ShouldPause ? 0 : 1;
            tutorialPanel.UpdatePanel(steps[stepIndex], currentTasks, taskFinished);
            EnableInputs();
        }

        private void EnableInputs()
        {
            foreach (var action in inputActions)
            {
                action?.Enable();
            }
        }

        private void DisableInputs()
        {
            foreach (var action in inputActions)
            {
                action?.Disable();
            }
        }

        private void OnTutorialEventEmitted(TutorialEvent tutorialEvent)
        {
            if (stepIndex >= steps.Count || currentTasks.Count <= 0) return;
            
            for (var j = 0; j < currentTasks.Count; j++)
            {
                var task = currentTasks[j];

                if (task == null || task.TaskType != TaskType.Event || task.TutorialEvent != tutorialEvent || taskFinished[j]) continue;
                
                
                taskFinished[j] = true;
                finishedTasks++;
                Debug.Log($"Task {j} completed: {task.TaskType}");
                tutorialPanel.UpdatePanel(steps[stepIndex], currentTasks, taskFinished);
                
                taskSoundEmitter.Play();
                
                break;
            }
        }
    }
}
