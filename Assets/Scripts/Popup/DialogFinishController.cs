using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DialogFinishController : MonoBehaviour {

    public Text txtScore;
    public Text txtBestScore;
    public Text txtTitle;

    public Button btnPlayAgain;
    public Button btnBackMenu;

    private IObservable<Unit> btnPlayAgainClickStream;
    private IObservable<Unit> btnBackMenuClickStream;
    
    // Observable 削除管理
    private CompositeDisposable disposables = new CompositeDisposable();

    private PopupObject popupObject;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IObservable<Unit> GetBtnPlayeAgainClickStream()
    {
        return btnPlayAgainClickStream;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IObservable<Unit> GetBtnBackMenuClickStream()
    {
        return btnBackMenuClickStream;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="score"></param>
    /// <param name="bestScore"></param>
    public void SetData(int score, int bestScore, string title = "Game Over!")
    {
        txtScore.text = string.Format("Score\n {0}", score);
        txtBestScore.text = string.Format("Best\n {0}", bestScore);
        txtTitle.text = title; 

        btnPlayAgainClickStream = btnPlayAgain.OnClickAsObservable();
        btnBackMenuClickStream = btnBackMenu.OnClickAsObservable();

        Observable.Merge(btnPlayAgainClickStream, btnBackMenuClickStream).Subscribe(_ =>
        {
            if (popupObject != null)
            {
                popupObject.Close();
            }
            disposables.Clear();
        }).AddTo(disposables);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static DialogFinishController Create()
    {
        PopupObject po = PopupObject.Create();
        GameObject go = po.SetDialogContent(Resources.Load<GameObject>("Prefab/Popup/dialogFinishContent"));
        DialogFinishController dialogFinishController = go.GetComponent<DialogFinishController>();
        dialogFinishController.popupObject = po;
        return dialogFinishController;
    }
}
