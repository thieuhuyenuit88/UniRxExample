using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnswerController : MonoBehaviour {

    public Text answerText;
    public Button answerButton;

    private AnswerData answerData;
    private IObservable<bool> answerClickStream;

    /// <summary>
    /// 質問データ取得
    /// </summary>
    /// <returns></returns>
    public AnswerData GetAnswerData()
    {
        return answerData;
    }

    /// <summary>
    /// 質問答えストリーム取得
    /// </summary>
    /// <returns></returns>
    public IObservable<bool> GetAnswerClickStream()
    {
        return answerClickStream;
    }

    /// <summary>
    /// データ設定
    /// </summary>
    /// <param name="data"></param>
    public void SetData(AnswerData data)
    {
        answerData = data;
        answerText.text = answerData.answerContent;

        //質問の答えストリーム作成
        answerClickStream = answerButton.OnClickAsObservable().Select(_ => answerData.isCorrect);
    }
}
