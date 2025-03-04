using UnityEngine;

namespace Tutorial
{
    [CreateAssetMenu(fileName = "TutorialEvent", menuName = "Tutorial/TutorialEvent")]
    public class TutorialEvent : ScriptableObject
    {
        [SerializeField] private string _name;
        public string Name => _name;
    }
}
