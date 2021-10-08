using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.ScriptableObjects
{
    public interface IScriptableCopy<T> where T : ScriptableObject
    {
        void Copy(T copyFrom, T refCopyTo);
    }
}