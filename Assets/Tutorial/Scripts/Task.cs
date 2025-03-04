using System;
using Resources;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tutorial
{
    [CreateAssetMenu(fileName = "Task", menuName = "Tutorial/Task")]
    public class Task : ScriptableObject
    {
        [SerializeField] private string _text;
        public string Text => _text;
        
        [SerializeField] private TaskType _taskType = TaskType.None;
        public TaskType TaskType => _taskType;

        //Input
        [Space(20)]
        [SerializeField] private InputAction _inputAction;
        public InputAction InputAction => _inputAction;

        [SerializeField] private float _pressTime = 0f;
        public float PressTime => _pressTime;
        
        //Event
        [Space(20)]
        [SerializeField] private TutorialEvent _tutorialEvent;
        public TutorialEvent TutorialEvent => _tutorialEvent;

        //ResourceAmount
        [Space(20)]
        
        [SerializeField] private ResourceType _resourceType = ResourceType.None;
        public ResourceType ResourceType => _resourceType;
        [SerializeField] private int _resourceAmount = 0;
        public int ResourceAmount => _resourceAmount;
        

    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(Task))]
    public class TaskEditor : Editor
    {
        private SerializedProperty textProperty;
        private SerializedProperty taskTypeProperty;
        
        private SerializedProperty inputActionProperty;
        private SerializedProperty pressTimeProperty;
        
        private SerializedProperty eventProperty;
        
        private SerializedProperty resourceTypeProperty;
        private SerializedProperty resourceAmountProperty;
        

        private void OnEnable()
        {
            textProperty = serializedObject.FindProperty("_text");
            taskTypeProperty = serializedObject.FindProperty("_taskType");
            
            inputActionProperty = serializedObject.FindProperty("_inputAction");
            pressTimeProperty = serializedObject.FindProperty("_pressTime");

            eventProperty = serializedObject.FindProperty("_tutorialEvent");
            
            resourceTypeProperty = serializedObject.FindProperty("_resourceType");
            resourceAmountProperty = serializedObject.FindProperty("_resourceAmount");
        }

        public override void OnInspectorGUI()
        {
            
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(textProperty);
            EditorGUILayout.PropertyField(taskTypeProperty);

            switch ((TaskType)taskTypeProperty.enumValueIndex)
            {
                case TaskType.Input:
                {
                    EditorGUILayout.PropertyField(inputActionProperty);
                    EditorGUILayout.PropertyField(pressTimeProperty);
                    break;
                }
                case TaskType.Event:
                {
                    EditorGUILayout.PropertyField(eventProperty);
                    break;
                }
                case TaskType.ResourceAmount:
                {
                    EditorGUILayout.PropertyField(resourceTypeProperty);
                    EditorGUILayout.PropertyField(resourceAmountProperty);
                    break;
                }
                case TaskType.None:
                    break;
                default:
                    throw new ArgumentException("Unknown TaskType " + taskTypeProperty.enumValueIndex + " for " + name);
            }

            
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
