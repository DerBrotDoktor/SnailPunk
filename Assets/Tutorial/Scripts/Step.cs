using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tutorial
{
    [CreateAssetMenu(fileName = "Step", menuName = "Tutorial/Step")]
    public class Step : ScriptableObject
    {
        [SerializeField] private EventReference _voiceOver;
        public EventReference VoiceOver => _voiceOver;
        
        [SerializeField] private bool _isFlavorText;
        public bool IsFlavorText => _isFlavorText;
        
        //FlavorText
        [Space(20)]
        [SerializeField] private string _flavorText;
        public string FlavorText => _flavorText;
        
        //Task
        [Space(20)]
        [SerializeField] private string _title;
        public string Name => _title;

        [SerializeField] private bool _shouldPause;
        public bool ShouldPause => _shouldPause;
        
        [SerializeField] private List<Task> _tasks = new List<Task>();
        public List<Task> Tasks => _tasks;
        
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(Step))]
    public class StepEditor : Editor
    {
        private SerializedProperty voiceOverProperty;
        private SerializedProperty isFlavorProperty;
        
        private SerializedProperty flavorTextProperty;
        
        private SerializedProperty titleProperty;
        private SerializedProperty shouldPauseProperty;
        private SerializedProperty tasksProperty;

        private void OnEnable()
        {
            voiceOverProperty = serializedObject.FindProperty("_voiceOver");
            isFlavorProperty = serializedObject.FindProperty("_isFlavorText");
            
            flavorTextProperty = serializedObject.FindProperty("_flavorText");
            
            titleProperty = serializedObject.FindProperty("_title");
            shouldPauseProperty = serializedObject.FindProperty("_shouldPause");
            tasksProperty = serializedObject.FindProperty("_tasks");
        }

        public override void OnInspectorGUI()
        {
            
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(voiceOverProperty);
            EditorGUILayout.PropertyField(isFlavorProperty);

            if (isFlavorProperty.boolValue)
            {
                EditorGUILayout.PropertyField(flavorTextProperty);
            }
            else
            {
                EditorGUILayout.PropertyField(titleProperty);
                EditorGUILayout.PropertyField(shouldPauseProperty);
                EditorGUILayout.PropertyField(tasksProperty);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
