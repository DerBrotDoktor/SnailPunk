using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
namespace Editor.Window
{
    public class TimeControlWindow : EditorWindow
    {

        private const float SmallSpacing = 5f;


        [MenuItem("SnailPunk/TimeControlWindow")]
        public static void ShowWindow()
        {
            GetWindow<TimeControlWindow>("TimeControlWindow");
        }
        private void OnGUI()
        {
            try
            {
                GUIStyle labelStyle = EditorStyles.whiteLabel;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.fontSize = 16;
                
                GUILayout.Label("Time Control", labelStyle);
                GUILayout.Space(SmallSpacing);
                GUILayout.Label("Current Time Scale: " + Game.TimeScale, labelStyle);
                GUILayout.Space(SmallSpacing);

                if (GUILayout.Button("0", GUILayout.Height(25f)))
                {
                    SetGameSpeed(0);
                }
                GUILayout.Space(SmallSpacing);
                if (GUILayout.Button("1", GUILayout.Height(25f)))
                {
                    SetGameSpeed(1);
                }
                GUILayout.Space(SmallSpacing);
                if (GUILayout.Button("2", GUILayout.Height(25f)))
                {
                    SetGameSpeed(2);
                }
                GUILayout.Space(SmallSpacing);
                if (GUILayout.Button("5", GUILayout.Height(25f)))
                {
                    SetGameSpeed(5);
                }
                GUILayout.Space(SmallSpacing);
                if (GUILayout.Button("10", GUILayout.Height(25f)))
                {
                    SetGameSpeed(10);
                }
                

            }
            catch (System.Exception e)
            {
                Debug.Log("UtilityWindow Error");
                Debug.LogException(e);
            }
        }

        private void SetGameSpeed(int speed)
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("Set time scale to " + speed);
                Game.TimeScale = speed;
            }
        }
    }
}
#endif
