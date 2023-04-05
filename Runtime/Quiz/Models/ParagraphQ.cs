using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Quizzes
{
    [System.Serializable]
    public class ParagraphQ : QuestionBase
    {
        /// <summary>
        /// If true means than we use commentary as positive commentary and commentaryNegative as negative commentary
        /// Even has not correct answer it has a score added by IsCorrect flag
        /// </summary>
        public override bool HasCorrectAnswer => false;

        public ParagraphQ()
        {
            type = QuestionType.Paragraph;
        }
    }

    [System.Serializable]
    public class ParagraphA : AnswerBase
    {
        public string paragraph;
    }
}