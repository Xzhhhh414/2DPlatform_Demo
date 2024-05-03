using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

[InitializeOnLoad]
public class ToolBarExtention
{
    static ToolBarExtention()
    {
        ToolbarExtender.RightToolbarGUI.Add(TimeScaleToolBar);
        ToolbarExtender.LeftToolbarGUI.Add(StartGameToolBar);
    }

    static string preScenePath = "";
    private static void StartGameToolBar()
    {

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("开始游戏", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            if (!SceneManager.GetActiveScene().name.Equals("GameplayScene"))
            {
                EditorApplication.playModeStateChanged += OnStartGameButtonClicked;
                preScenePath = SceneManager.GetActiveScene().path;
                EditorSceneManager.OpenScene("Assets/Scenes/GameplayScene.unity");
            }
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
        if (GUILayout.Button("开始界面", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            if (!SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
            {
                EditorApplication.playModeStateChanged += OnStartGameButtonClicked;
                preScenePath = SceneManager.GetActiveScene().path;
                EditorSceneManager.OpenScene("Assets/Scenes/MainMenuScene.unity");
            }
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
    }
    private static void OnStartGameButtonClicked(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            EditorSceneManager.OpenScene(preScenePath);

            EditorApplication.playModeStateChanged -= OnStartGameButtonClicked;
        }
    }
    public static bool isMinDamage;
    private static void TimeScaleToolBar()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("TimeScale:", GUILayout.Width(70));
        //Time.timeScale = EditorGUILayout.FloatField(Time.timeScale, GUILayout.Width(40));
        if (GUILayout.Button("1", EditorStyles.toolbarButton, GUILayout.Width(30)))
        {
            BattleTestManager.Instance.TimeScale = 1f;
            BattleTestManager.Instance.GMTimeScale();
            Application.runInBackground = true;
        }
        if (GUILayout.Button("0.2", EditorStyles.toolbarButton, GUILayout.Width(30)))
        {
            BattleTestManager.Instance.TimeScale = 0.2f;
            BattleTestManager.Instance.GMTimeScale();
            Application.runInBackground = true;
        }
        if (GUILayout.Button("0.5", EditorStyles.toolbarButton, GUILayout.Width(30)))
        {
            BattleTestManager.Instance.TimeScale = 0.5f;
            BattleTestManager.Instance.GMTimeScale();
            Application.runInBackground = true;
        }
        GUI.color = isMinDamage ? Color.green : Color.white;
        if (GUILayout.Button("最小伤害", EditorStyles.toolbarButton, GUILayout.Width(70)))
        {
            isMinDamage = !isMinDamage;
            BattleTestManager.Instance.isMinDamage = isMinDamage;
        }
        GUILayout.EndHorizontal();
    }
}
