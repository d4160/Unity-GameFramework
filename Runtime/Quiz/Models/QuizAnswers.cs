using d4160.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using static PlasticGui.WorkspaceWindow.CodeReview.Summary.CommentSummaryData;

namespace d4160.Quiz_
{
    [System.Serializable]
    public class QuizAnswers
    {
        public string quizId;
        public ulong clientId;
        public string playerId;
        public string playerName;
        public List<AnswerBase> answers = new();
        public int totalScore;
        public int maxScore;

        public void AddAnswer(AnswerBase answer)
        {
            if (!answers.Contains(answer))
            {
                answer.Index = answers.Count;
                answers.Add(answer);
            }
        }

        public T AddAnswer<T>(QuestionType type) where T : AnswerBase
        {
            AnswerBase answer = default;
            switch (type)
            {
                case QuestionType.ShortAnswer:
                    answer = new ShortAnswer();
                    break;
                case QuestionType.MultipleChoices:
                    answer = new MultipleChoicesA();
                    break;
                case QuestionType.Dropdown:
                    answer = new DropdownA();
                    break;
                case QuestionType.MultipleSelections:
                    answer = new MultipleSelectionsA();
                    break;
                case QuestionType.Paragraph:
                    answer = new ParagraphA();
                    break;
                case QuestionType.LinealScale:
                    answer = new LinealScaleA();
                    break;
                default:
                    break;
            }

            AddAnswer(answer);

            return answer as T;
        }

        public void RemoveAnswer(int index)
        {
            for (int i = 0; i < answers.Count; i++)
            {
                if (answers[i].Index == index)
                {
                    answers.RemoveAt(i);
                    break;
                }
            }
        }

        public T GetAnswer<T>(int index) where T : AnswerBase
        {
            if (answers.IsValidIndex(index))
            {
                return answers[index] as T;
            }

            return default;
        }

        public void ValidateAnswer(Quiz quiz, int index)
        {
            if (quiz != null && quiz.questions.IsValidIndex(index) && answers.IsValidIndex(index))
            {
                quiz.questions[index].ValidateAnswer(answers[index]);
            }
        }

        public void ValidateAnswersAndCalculateTotalScore(Quiz quiz)
        {
            if (quiz != null && quiz.questions.Count == answers.Count)
            {
                for (int i = 0; i < quiz.questions.Count; i++)
                {
                    quiz.questions[i].ValidateAnswer(answers[i]);
                }

                CalculateTotalScore(quiz);
            } 
        }

        public void CalculateTotalScore(Quiz quiz)
        {
            int maxScore = quiz.GetMaxScore();
            int totalScore = 0;
            for (int i = 0; i < answers.Count; i++)
            {
                totalScore += answers[i].Score;
            }

            this.maxScore = maxScore;
            this.totalScore = totalScore;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Quiz.Settings);
        }

        public static QuizAnswers FromJson(string json)
        {
            return JsonConvert.DeserializeObject<QuizAnswers>(json, Quiz.Settings);
        }
    }
}
