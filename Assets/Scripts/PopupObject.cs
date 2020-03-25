using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PopupObject : MonoBehaviour {

    public Transform dialogParent;
    public GameObject bgMaskObject;

    public enum TWEEN_TYPE
    {
        ZOOM_IN,
        MAX
    }
    private TWEEN_TYPE tweenType;

    public Subject<Unit> openCompletedCallback {
        get; set;
    }

    public Subject<Unit> closeCompletedCallback
    {
        get; set;
    }

    private void Start()
    {
        openCompletedCallback = new Subject<Unit>();
        closeCompletedCallback = new Subject<Unit>();
    }

    /// <summary>
    /// 開く
    /// </summary>
    /// <param name="_tweenType"></param>
    protected void Open(TWEEN_TYPE _tweenType = TWEEN_TYPE.ZOOM_IN)
    {
        tweenType = _tweenType;
        bgMaskObject.SetActive(false);
        gameObject.GetComponent<CanvasGroup>().interactable = false;

        switch (tweenType)
        {
            case TWEEN_TYPE.ZOOM_IN:
            default:
                dialogParent.transform.localScale = Vector3.zero;
                LeanTween.scale(dialogParent.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack).
                    setOnComplete(() =>
                    {
                        bgMaskObject.SetActive(true);
                        gameObject.GetComponent<CanvasGroup>().interactable = true;
                        openCompletedCallback.OnNext(Unit.Default);
                    });
                break;
        }
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    public void Close()
    {
        gameObject.GetComponent<CanvasGroup>().interactable = false;

        if (LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject, true);
        }

        switch (tweenType)
        {
            case TWEEN_TYPE.ZOOM_IN:
            default:
                LeanTween.scale(dialogParent.gameObject, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInBack).
                setOnComplete(() =>
                {
                    closeCompletedCallback.OnNext(Unit.Default);
                    PopupDestroy();
                });
                break;
            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_dialogContent"></param>
    /// <returns></returns>
    public GameObject SetDialogContent(GameObject _dialogContent)
    {
        GameObject go = Instantiate(_dialogContent, dialogParent, false);
        return go;
    }

    private void PopupDestroy()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// ポップアップcanvasを探す関数
    /// </summary>
    /// <param name="_popupCanvasName"></param>
    /// <returns></returns>
    protected Transform FindPopupCanvas(string _popupCanvasName = "PopupCanvas")
    {
        GameObject popupCanvas = GameObject.Find(_popupCanvasName);
        if (popupCanvas == null)
        {
            GameObject originPrefab = Resources.Load<GameObject>("Prefab/Popup/" + _popupCanvasName);
            popupCanvas = Instantiate(originPrefab);
            popupCanvas.name = originPrefab.name;
            popupCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        }

        return popupCanvas.transform;
    }

    private void SetParent()
    {
        transform.SetParent(FindPopupCanvas(), false);
    }

    /// <summary>
    /// ポップアップオブジェクト作成
    /// </summary>
    /// <param name="_tweenType"></param>
    /// <returns></returns>
    public static PopupObject Create(TWEEN_TYPE _tweenType = TWEEN_TYPE.ZOOM_IN)
    {
        GameObject originPrefab = Resources.Load<GameObject>("Prefab/Popup/PopupObject");
        PopupObject popupObject = Instantiate(originPrefab).GetComponent<PopupObject>();
        popupObject.Open(_tweenType);
        popupObject.SetParent();
        return popupObject;
    }
}
