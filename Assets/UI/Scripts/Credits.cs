using UnityEngine;

namespace UI
{
    public class Credits : MonoBehaviour
    {

        private void Awake()
        {
            Time.timeScale = 1f;
        }
        public void OnBackButtonClicked()
        {
            FindObjectOfType<SceneLoader>().LoadMainMenu();
        }
    }
}
