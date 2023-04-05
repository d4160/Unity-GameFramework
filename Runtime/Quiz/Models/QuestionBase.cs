using Newtonsoft.Json;
using UnityEngine;

namespace d4160.Quizzes
{
    [System.Serializable]
    public abstract class QuestionBase
    {
        public QuestionType type;
        public string title;
        public bool required;
        public string imageUrl;
        public int score;
        [TextArea]
        public string commentary;
        [TextArea]
        public string commentaryNegative;

        [HideInInspector] public int index;

        [JsonIgnore]
        public int Index
        {
            get => index;
            set => index = value;
        }

        /// <summary>
        /// If true means than we use commentary as positive commentary and commentaryNegative as negative commentary
        /// Even has not correct answer it has a score added by IsCorrect flag
        /// </summary>
        [JsonIgnore]
        public virtual bool HasCorrectAnswer { get; } = false;

        public virtual int ValidateAnswer(AnswerBase answer)
        {
            answer.Answered = true;
            answer.Score = 0;
            answer.IsCorrect = true;
            return 0;
        }

        public T GetAs<T>() where T : QuestionBase
        {
            switch (type)
            {
                case QuestionType.ShortAnswer:
                    return this as ShortAnswerQ as T;
                case QuestionType.MultipleChoices:
                    return this as MultipleChoicesQ as T;
                case QuestionType.Dropdown:
                    return this as DropdownQ as T;
                case QuestionType.MultipleSelections:
                    return this as MultipleSelectionsQ as T;
                case QuestionType.Paragraph:
                    return this as ParagraphQ as T;
                case QuestionType.LinealScale:
                    return this as LinealScaleQ as T;
                default:
                    return this as T;
            }
        }
    }

    public enum QuestionType
    {
        ShortAnswer,
        MultipleChoices,
        Dropdown,
        MultipleSelections,
        Paragraph,
        LinealScale
    }

    [System.Serializable]
    public abstract class AnswerBase
    {
        [HideInInspector] public bool answered;
        [HideInInspector] public bool isCorrect;
        [HideInInspector] public int score;
        [HideInInspector] public int index;
        [HideInInspector] public QuestionType type;

        [JsonIgnore]
        public bool Answered
        {
            get => answered;
            set => answered = value;
        }

        [JsonIgnore]
        public bool IsCorrect
        {
            get => isCorrect;
            set => isCorrect = value;
        }

        [JsonIgnore]
        public int Score
        {
            get => score;
            set => score = value;
        }

        [JsonIgnore]
        public int Index
        {
            get => index;
            set => index = value;
        }
    }
}