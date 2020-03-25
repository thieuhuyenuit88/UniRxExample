using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MainGameController : MonoBehaviour
{

    public Text questionText;         //質問内容テキスト
    public Text bestScoreText;        //一番高いスコア
    public Text currentScoreText;     //現在のスコア
    public Text currentTimeText;      //残り時間（秒）
    public Slider timeProgressSlider; //スライダーで残り時間表す 

    public GameObject buttonAnswerPrefab;
    public Transform buttonsAnswerParent;

    private ButtonAnswerPool buttonAnswerPool;

    private RoundData currentRoundData;

    private List<QuestionData> listQuestions;

    private IntReactiveProperty currentQuestionIndex;
    private StringReactiveProperty questionString;
    private IntReactiveProperty scoreReactive;
    private IReadOnlyReactiveProperty<int> bestScore;

    private List<ButtonAnswerController> listAnswerButtonObjects = new List<ButtonAnswerController>();

    private float timeRemain;
    private bool isPlaying;

    // Observable 削除管理
    private CompositeDisposable disposables = new CompositeDisposable();

    private DialogFinishController dialogFinishController = null;

    // Use this for initialization
    void Start()
    {

        ButtonAnswerController btnAnswerController = buttonAnswerPrefab.GetComponent<ButtonAnswerController>();
        if (btnAnswerController != null)
        {
            buttonAnswerPool = new ButtonAnswerPool(btnAnswerController, buttonsAnswerParent);
        }
        if (buttonAnswerPool == null)
        {
            Debug.Log("Cant create button answer pool!!!");
            return;
        }

        currentRoundData = DataManager.Instance.GetRoundData();
        listQuestions = currentRoundData.questions.ShuffleList();
        currentQuestionIndex = new IntReactiveProperty(0);
        isPlaying = true;

        // 質問コンテンツ設定
        questionString = new StringReactiveProperty(string.Empty);
        questionString.SubscribeToText(questionText).AddTo(gameObject);

        // 点数をフォローする
        scoreReactive = new IntReactiveProperty();
        scoreReactive.Subscribe(value =>
        {
            currentScoreText.text = string.Format("Score: {0}", value);
            AnimatedObject(currentScoreText.gameObject);
        }).AddTo(gameObject);

        bestScore = scoreReactive.Scan(DataManager.Instance.LoadBestScore(), Mathf.Max).ToReactiveProperty();
        bestScore.Subscribe(value =>
        {
            bestScoreText.text = string.Format("Best: {0}", value);
            AnimatedObject(bestScoreText.gameObject);
        }).AddTo(gameObject);

        //　残り時間設定
        timeRemain = currentRoundData.timeLimit;
        //var noPlay = Observable.EveryUpdate().Where(_ => isPlaying == false);
        Observable.EveryUpdate()
            .Where(_ => isPlaying == true)
            //.TakeUntil(noPlay)
            //.RepeatUntilDestroy()
            .Subscribe(_ =>
        {
            if (timeRemain > 0f)
            {
                timeRemain -= Time.deltaTime;
            }
            else
            {
                timeRemain = 0f;
                isPlaying = false;
                Debug.Log("TIME UP!!!");
                ShowDialogFinish();
            }

            timeProgressSlider.value = timeRemain / (float)currentRoundData.timeLimit;
            currentTimeText.text = Mathf.CeilToInt(timeRemain).ToString();

        }).AddTo(gameObject);

        // 質問及び答え表示      
        currentQuestionIndex.Subscribe(value =>
        {
            ShowQuestionAndAnswers(value);
        });
    }

    /// <summary>
    /// 
    /// </summary>
    protected void ShowQuestionAndAnswers(int _questionIndex)
    {
        if (_questionIndex < 0 || _questionIndex >= listQuestions.Count) return;

        QuestionData currentQuestionData = listQuestions[_questionIndex];
        questionString.Value = currentQuestionData.questionContent;
        timeRemain = currentRoundData.timeLimit;

        // 答えボタン設定
        RemoveAnswerButtons();
        for (int i = 0; i < currentQuestionData.answers.Count; i++)
        {
            var buttonAnswer = buttonAnswerPool.Rent();
            listAnswerButtonObjects.Add(buttonAnswer);
            buttonAnswer.SetData(currentQuestionData.answers[i]);

            var answerClickStream = buttonAnswer.GetAnswerClickStream();
            answerClickStream.Subscribe(answer =>
            {
                if (answer)
                {
                    Debug.Log("CORRECT!!!");
                    scoreReactive.Value += currentRoundData.pointAddIfCorrect;
                    if (currentQuestionIndex.Value + 1 < currentRoundData.questions.Count)
                    {
                        currentQuestionIndex.Value++;
                    }
                    else
                    {
                        Debug.Log("END ROUND!!!");
                        isPlaying = false;
                        ShowDialogFinish("End Round!");
                    }
                }
                else
                {
                    Debug.Log("WRONG!!!");
                    isPlaying = false;
                    ShowDialogFinish();
                }
            }).AddTo(disposables);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void ShowDialogFinish(string titleText = "Game Over!")
    {
        dialogFinishController = DialogFinishController.Create();
        dialogFinishController.SetData(scoreReactive.Value, bestScore.Value, titleText);
        DataManager.Instance.SaveBestScore(bestScore.Value);

        var disposablesLocal = new CompositeDisposable();
        dialogFinishController.GetBtnPlayeAgainClickStream()
            .Subscribe(_ =>
            {
                Debug.Log("Play Again!");
                if (!disposablesLocal.IsDisposed)
                {
                    disposablesLocal.Dispose();
                }
                currentQuestionIndex.Value = -1;
                currentQuestionIndex.Value = 0;
                timeRemain = currentRoundData.timeLimit;
                isPlaying = true;
                scoreReactive.Value = 0;
                listQuestions = listQuestions.ShuffleList();
            }).AddTo(disposablesLocal);

        dialogFinishController.GetBtnBackMenuClickStream()
            .Subscribe(_ =>
            {
                Debug.Log("Back to Main Menu!");
                if (!disposablesLocal.IsDisposed)
                {
                    disposablesLocal.Dispose();
                }
                SceneManager.LoadScene("MainMenu");
            }).AddTo(disposablesLocal);
    }

    /// <summary>
    /// 
    /// </summary>
    protected void RemoveAnswerButtons()
    {
        disposables.Clear();
        while (listAnswerButtonObjects.Count > 0)
        {
            buttonAnswerPool.Return(listAnswerButtonObjects[0]);
            listAnswerButtonObjects.RemoveAt(0);
        }
    }

    /// <summary>
    /// GameObjectのscaleアニメーション関数
    /// </summary>
    /// <param name="_go"></param>
    void AnimatedObject(GameObject _go)
    {
        LeanTween.scale(_go, Vector3.one, 0.2f)
            .setFrom(Vector3.one * 0.5f)
            .setEase(LeanTweenType.easeOutBack);
    }

}

static class MyExtensions
{
    /// <summary>
    /// 配列シャッフル
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static List<T> ShuffleList<T>(this List<T> list)
    {
        int count = list.Count;
        for (int i = count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i);
            T tempValue = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = tempValue;
        }

        return list;
    }
}
