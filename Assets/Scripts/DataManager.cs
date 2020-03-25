using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : Singleton<DataManager> {
    public List<RoundData> allRoundData = new List<RoundData>();

    private string gameDataFileName = "data.json";

    private int currentRoundIndex;

    public void Init()
    {
        LoadGameData();
        currentRoundIndex = 0;
    }

    public void SetRoundIndex(int roundIndex)
    {
        currentRoundIndex = roundIndex;
    }

    public RoundData GetRoundData()
    {
        return allRoundData[currentRoundIndex];
    }

    private void LoadGameData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);

        if (File.Exists(filePath))
        {
            string dataTextJson = File.ReadAllText(filePath);

            GameData loadedData = JsonUtility.FromJson<GameData>(dataTextJson);

            allRoundData = loadedData.rounds;
        }
        else
        {
            Debug.Log("Cant not load game data!");
            GameData loadedData = new GameData();
            allRoundData = loadedData.rounds;
        }
    }

    public int LoadBestScore()
    {
        if (allRoundData[currentRoundIndex] == null) return 0;
        string key = "BestScore" + allRoundData[currentRoundIndex].roundName;
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetInt(key);
        }

        return 0;
    }

    public void SaveBestScore(int bestScore)
    {
        if (allRoundData[currentRoundIndex] != null)
        {
            PlayerPrefs.SetInt("BestScore" + allRoundData[currentRoundIndex].roundName, bestScore);
        }
    }
}
