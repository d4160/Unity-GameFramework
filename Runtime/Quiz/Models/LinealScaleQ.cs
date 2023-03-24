using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Quiz_
{
    [System.Serializable]
    public class LinealScaleQ : QuestionBase
    {
        public int minValue;
        public int maxValue;

        /// <summary>
        /// If true means than we use commentary as positive commentary and commentaryNegative as negative commentary
        /// Even has not correct answer it has a score added by IsCorrect flag
        /// </summary>
        public override bool HasCorrectAnswer => false;

        public LinealScaleQ()
        {
            type = QuestionType.LinealScale;
        }
    }

    [System.Serializable]
    public class LinealScaleA : AnswerBase
    {
        public int value;
    }
}