using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GameDataEditor : EditorWindow {

    public GameData gameData = new GameData();

    private string gameDataProjectFilePath = "/StreamingAssets/data.json";

    private Vector2 beginScrollPos = Vector2.zero;

    [MenuItem("Window/Clear Best Score")]
    static void ClearBestScore()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Clear all best score");
    }

    [MenuItem("Window/Game Data Editor")]
    static void ShowGameDataEditor()
    {
        EditorWindow.GetWindow(typeof(GameDataEditor)).Show();
    }

    private void OnGUI()
    {
        beginScrollPos = EditorGUILayout.BeginScrollView(beginScrollPos);
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty serializedProperty = serializedObject.FindProperty("gameData");
        EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Save data"))
        {
            SaveGameData(gameData);
        }

        if (GUILayout.Button("Load data"))
        {
            LoadGameData();
        }
    }

    private void LoadGameData()
    {
        string filePath = Application.dataPath + gameDataProjectFilePath;

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(dataAsJson);
        }
        else
        {
            gameData = new GameData();
            SaveGameData(gameData);
        }
        Debug.Log("Data Loaded!!");
    }

    private void SaveGameData(GameData _data)
    {
        string dataAsJson = JsonUtility.ToJson(_data);

        string filePath = Application.dataPath + gameDataProjectFilePath;
        File.WriteAllText(filePath, dataAsJson);
        Debug.Log("Data Saved!!");
    }
}
