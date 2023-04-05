using d4160.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Quizzes
{
    [System.Serializable]
    public class Quiz 
    {
        public string id;
        public string playerId;
        public ulong clientId;
        public List<QuestionBase> questions = new();

        [JsonIgnore]
        public List<QuizAnswers> QuizAnswers { get; private set; } = new();

        public static readonly JsonSerializerSettings Settings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public Quiz()
        {
            id = System.Guid.NewGuid().ToString();
        }

        public void AddQuestion(QuestionBase question)
        {
            if (!questions.Contains(question))
            {
                question.Index = questions.Count;
                questions.Add(question);
            }
        }

        public T AddQuestion<T>(QuestionType type) where T : QuestionBase
        {
            QuestionBase question = default;
            switch (type)
            {
                case QuestionType.ShortAnswer:
                    question = new ShortAnswerQ();
                    break;
                case QuestionType.MultipleChoices:
                    question = new MultipleChoicesQ();
                    break;
                case QuestionType.Dropdown:
                    question = new DropdownQ();
                    break;
                case QuestionType.MultipleSelections:
                    question = new MultipleSelectionsQ();
                    break;
                case QuestionType.Paragraph:
                    question = new ParagraphQ();
                    break;
                case QuestionType.LinealScale:
                    question = new LinealScaleQ();
                    break;
                default:
                    break;
            }

            AddQuestion(question);

            return question as T;
        }

        public void RemoveQuestion(int index)
        {
            bool removed = false;
            for (int i = 0; i < questions.Count; i++)
            {
                if (questions[i].Index == index)
                {
                    questions.RemoveAt(i);
                    removed = true;
                    break;
                }
            }

            if (removed)
                RegenerateQuestionIndexes();
        }

        public void SwapQuestion(int fromIndex, int toIndex)
        {
            if (questions.IsValidIndex(fromIndex) && questions.IsValidIndex(toIndex)) 
            {
                QuestionBase temp = questions[fromIndex];
                questions[fromIndex] = questions[toIndex];
                questions[toIndex] = temp;

                RegenerateQuestionIndexes();
            }
        }

        public T GetQuestion<T>(int index) where T : QuestionBase
        {
            if (questions.IsValidIndex(index))
            {
                return questions[index] as T;
            }

            return default;
        }

        public int GetMaxScore()
        {
            int maxScore = 0;
            for (int i = 0; i < questions.Count; i++)
            {
                maxScore += questions[i].score;
            }

            return maxScore;
        }

        private void RegenerateQuestionIndexes() 
        {
            for (int i = 0; i < questions.Count; i++)
            {
                questions[i].Index = i;
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Settings);
        }

        public static Quiz FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Quiz>(json, Settings);
        }
    }
}
