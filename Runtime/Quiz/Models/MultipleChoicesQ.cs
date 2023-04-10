using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Quizzes
{
    [System.Serializable]
    public class MultipleChoicesQ : QuestionBase
    {
        public List<string> choices = new();
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
                    answer.Scored = true;
                    answer.Score = score;
                    answer.IsCorrect = true;

                    return score;
                }
            }

            answer.Scored = true;
            answer.Score = 0;
            answer.IsCorrect = false;
            return 0;
        }

        public bool ValidateChoices()
        {
            for (int i = 0; i < choices.Count; i++)
            {
                if (string.IsNullOrEmpty(choices[i]) || string.IsNullOrWhiteSpace(choices[i])) 
                {
                    return false;
                }
            }
            return true;
        }
    }

    [System.Serializable]
    public class MultipleChoicesA : AnswerBase
    {
        public int answer;

        public void Answer(int answer)
        {
            Answer();
            this.answer = answer;
        }
    }
}