using System;
using UnityEngine;

namespace Tutorial
{
    public class TutorialEventEmitter : MonoBehaviour
    {
        public static Action<TutorialEvent> TutorialEventEmitted;
        
        [SerializeField] private TutorialEvent tutorialEvent;
        
        public void Emit()
        {
            TutorialEventEmitted?.Invoke(tutorialEvent);
        }
    }
}
