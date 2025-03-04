using Codice.Client.Common;
using Drought;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Time = UnityEngine.Time;

#if UNITY_EDITOR
namespace Editor.Window
{
    public class SnailPunkDebugWindow : EditorWindow
    {

        private const float SmallSpacing = 5f;


        [MenuItem("SnailPunk/SnailPunkDebugWindow")]
        public static void ShowWindow()
        {
            GetWindow<SnailPunkDebugWindow>("SnailPunkDebugWindow");
        }
        private void OnGUI()
        {
            try
            {
                Color normalGuiColor = GUI.color;
                
                GUIStyle labelStyle = EditorStyles.whiteLabel;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.fontSize = 14;

                
                GUILayout.Space(SmallSpacing);
                GUILayout.Label("Scene Setup Tools", labelStyle);

                GUI.backgroundColor = Color.green;
                
                if (GUILayout.Button("Scene Setup - Game", GUILayout.Height(25f)))
                {
                    Debug.LogWarning("Editor: Scene Setup - Game");
                    EditorSceneManager.OpenScene("Assets/Scenes/Game/Main.unity", OpenSceneMode.Single);
                    EditorSceneManager.OpenScene("Assets/Scenes/Game/GameScene.unity", OpenSceneMode.Additive);
                    EditorSceneManager.OpenScene("Assets/Scenes/Game/InGameUI.unity", OpenSceneMode.Additive);
                    
                    EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByName("GameScene"));
                }
                
                if (GUILayout.Button("Scene Setup - Main Menu", GUILayout.Height(25f)))
                {
                    Debug.LogWarning("Editor: Scene Setup - Main Menu");
                    EditorSceneManager.OpenScene("Assets/Scenes/Game/Main.unity", OpenSceneMode.Single);
                    EditorSceneManager.OpenScene("Assets/Scenes/Game/MainMenu.unity", OpenSceneMode.Additive);
                    
                    EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByName("MainMenu"));
                }

                if (GUILayout.Button("Scene Setup - Settings Menu", GUILayout.Height(25f)))
                {
                    Debug.LogWarning("Editor: Scene Setup - Settings Menu");
                    EditorSceneManager.OpenScene("Assets/Scenes/Game/Main.unity", OpenSceneMode.Single);
                    EditorSceneManager.OpenScene("Assets/Scenes/Game/SettingsMenu.unity", OpenSceneMode.Additive);
                    
                    EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByName("SettingsMenu"));
                }

                GUILayout.Space(SmallSpacing);
                
                GUILayout.Label("Drought", labelStyle);
                GUI.backgroundColor = Color.yellow;
                
                if (GUILayout.Button("Start Drought", GUILayout.Height(25f)))
                {
                    Debug.LogWarning("Editor: Start Drought");
                    FindObjectOfType<DroughtController>().EditorStartDrought();
                }
                
                if (GUILayout.Button("Finish Drought", GUILayout.Height(25f)))
                {
                    Debug.LogWarning("Editor: Finish Drought");
                    FindObjectOfType<DroughtController>().EditorFinishDrought();
                }
                
                GUI.color = normalGuiColor;
            }
            catch (System.Exception e)
            {
                Debug.Log("UtilityWindow Error");
                Debug.LogException(e);
            }
        }
    }
}
#endif
