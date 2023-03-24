using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Quiz_
{
    [System.Serializable]
    public class ShortAnswerQ : QuestionBase
    {
        public string correctShortAnswer;

        /// <summary>
        /// If true means than we use commentary as positive commentary and commentaryNegative as negative commentary
        /// Even has not correct answer it has a score added by IsCorrect flag
        /// </summary>
        public override bool HasCorrectAnswer => true;

        public ShortAnswerQ()
        {
            type = QuestionType.ShortAnswer;
        }

        public override int ValidateAnswer(AnswerBase answer)
        {
            if (answer is ShortAnswer sa)
            {
                if (sa.answer == correctShortAnswer)
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
    public class ShortAnswer : AnswerBase
    {
        public string answer;
    }
}