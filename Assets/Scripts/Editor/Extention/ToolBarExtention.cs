using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

[InitializeOnLoad]
public class ToolBarExtention
{
    static ToolBarExtention()
    {
        ToolbarExtender.RightToolbarGUI.Add(TimeScaleToolBar);
        ToolbarExtender.LeftToolbarGUI.Add(StartGameToolBar);
    }

    private static void StartGameToolBar()
    {
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("开始游戏", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("GameplayScene"))
            {
                EditorSceneManager.OpenScene("Assets/Scenes/GameplayScene.unity");
            }
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
    }

    private static void TimeScaleToolBar()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("TimeScale:", GUILayout.Width(70));
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
        GUILayout.EndHorizontal();
    }
}
