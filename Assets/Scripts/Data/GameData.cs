using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AnswerData {
    public string answerContent;
    public bool isCorrect;
}

[System.Serializable]
public class QuestionData
{
    public string questionContent;
    public List<AnswerData> answers = new List<AnswerData>();
}

[System.Serializable]
public class RoundData
{
    public string roundName;
    public int timeLimit;
    public int pointAddIfCorrect;
    public List<QuestionData> questions = new List<QuestionData>();
}

[System.Serializable]
public class GameData
{
    public List<RoundData> rounds = new List<RoundData>();
}