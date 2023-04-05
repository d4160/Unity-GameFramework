using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Quizzes
{
    [System.Serializable]
    public class MultipleSelectionsQ : QuestionBase
    {
        public string[] choices;
        public int[] correctIndexes;

        /// <summary>
        /// If true means than we use commentary as positive commentary and commentaryNegative as negative commentary
        /// Even has not correct answer it has a score added by IsCorrect flag
        /// </summary>
        public override bool HasCorrectAnswer => true;

        public MultipleSelectionsQ()
        {
            type = QuestionType.MultipleSelections;
        }

        public override int ValidateAnswer(AnswerBase answer)
        {
            if (answer is MultipleSelectionsA msa)
            {
                if (msa.answers.Length == correctIndexes.Length)
                {
                    int correctsCount = 0;
                    for (int i = 0; i < msa.answers.Length; i++)
                    {
                        for (int j = 0; j < correctIndexes.Length; j++)
                        {
                            if (msa.answers[i] == correctIndexes[j])
                            {
                                correctsCount++;
                                break;
                            }
                        }
                    }

                    if (correctsCount == correctIndexes.Length)
                    {
                        answer.Answered = true;
                        answer.Score = score;
                        answer.IsCorrect = true;

                        return score;
                    }
                }
            }

            answer.Answered = true;
            answer.Score = 0;
            answer.IsCorrect = false;
            return 0;
        }
    }

    [System.Serializable]
    public class MultipleSelectionsA : AnswerBase
    {
        public int[] answers;
    }
}