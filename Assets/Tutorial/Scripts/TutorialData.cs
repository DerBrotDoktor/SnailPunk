using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    [CreateAssetMenu(fileName = "TutorialData", menuName = "Tutorial/TutorialData")]
    public class TutorialData : ScriptableObject
    {
        [SerializeField] private List<Step> _steps = new List<Step>();
        public List<Step> Steps => _steps;
    }
}