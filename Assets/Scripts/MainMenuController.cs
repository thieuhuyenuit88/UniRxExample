using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    public Transform buttonsParent;
    public GameObject btnOrigin;

    // Use this for initialization
    void Start () {
        DataManager.Instance.Init();
        btnOrigin.SetActive(false);

        List<RoundData> roundDatas = DataManager.Instance.allRoundData;
        foreach (var roundData in roundDatas)
        {
            if (roundData != null)
            {
                GameObject go = Instantiate(btnOrigin, buttonsParent, false);
                go.SetActive(true);
                go.GetComponentInChildren<Text>().text = roundData.roundName;
                Button btnRound = go.GetComponent<Button>();
                btnRound.onClick.AddListener(() => {
                    btnRound.onClick.RemoveAllListeners();
                    DataManager.Instance.SetRoundIndex(roundDatas.IndexOf(roundData));
                    SceneManager.LoadScene("MainGame");
                });
            }
        }
    }
}
