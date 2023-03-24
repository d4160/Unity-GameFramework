using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Quiz_
{
    [System.Serializable]
    public class MultipleChoicesQ : QuestionBase
    {
        public string[] choices;
        public int correctIndex;

        /// <summary>
        /// If true means than we use commentary as positive commentary and commentaryNegative as negative commentary
        /// Even has not correct answer it has a score added by IsCorrect flag
        /// </summary>
        public override bool HasCorrectAnswer => true;

        public MultipleChoicesQ()
        {
            type = QuestionType.MultipleChoices;
        }

        public override int ValidateAnswer(AnswerBase answer)
        {
            if (answer is MultipleChoicesA mca)
            {
                if (mca.answer == correctIndex)
                {
                    answer.Answered = true;
                    answer.Score = score;
                    answer.IsCorrect = true;

                    return score;
                }
            }

            answer.Answered = true;
            answer.Score = 0;
            answer.IsCorrect = false;
            return 0;
        }
    }

    [System.Serializable]
    public class MultipleChoicesA : AnswerBase
    {
        public int answer;
    }
}