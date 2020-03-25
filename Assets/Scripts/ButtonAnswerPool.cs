using System.Collections;
using System.Collections.Generic;
using UniRx.Toolkit;
using UnityEngine;

public class ButtonAnswerPool : ObjectPool<ButtonAnswerController> {

    readonly ButtonAnswerController buttonAnswerPrefab;
    readonly Transform hierachyParent;

    public ButtonAnswerPool(ButtonAnswerController prefab, Transform parent)
    {
        buttonAnswerPrefab = prefab;
        hierachyParent = parent;
    }

    protected override ButtonAnswerController CreateInstance()
    {
        ButtonAnswerController buttonAnswer = GameObject.Instantiate<ButtonAnswerController>(buttonAnswerPrefab, hierachyParent) as ButtonAnswerController;
        return buttonAnswer;
    }
}
