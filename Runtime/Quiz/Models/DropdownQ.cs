using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Quizzes
{
    [System.Serializable]
    public class DropdownQ : QuestionBase
    {
        public string[] choices;
        public int correctIndex;

        /// <summary>
        /// If true means than we use commentary as positive commentary and commentaryNegative as negative commentary
        /// Even has not correct answer it has a score added by IsCorrect flag
        /// </summary>
        public override bool HasCorrectAnswer => true;

        public DropdownQ()
        {
            type = QuestionType.Dropdown;
        }

        public override int ValidateAnswer(AnswerBase answer)
        {
            if (answer is DropdownA dda)
            {
                if (dda.answer == correctIndex)
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
    public class DropdownA : AnswerBase
    {
        public int answer;
    }
}