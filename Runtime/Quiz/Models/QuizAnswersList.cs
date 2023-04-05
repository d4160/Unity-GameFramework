using Newtonsoft.Json;
using System.Collections.Generic;

namespace d4160.Quizzes
{
    [System.Serializable]
    public class QuizAnswersList
    {
        public List<QuizAnswers> answersList = new();

        public void AddAnswer(QuizAnswers quizAnswer)
        {
            if (!answersList.Contains(quizAnswer))
            {
                answersList.Add(quizAnswer);
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Quiz.Settings);
        }

        public static QuizAnswersList FromJson(string json)
        {
            return JsonConvert.DeserializeObject<QuizAnswersList>(json, Quiz.Settings);
        }
    }
}
