using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using InspectInLine;

namespace d4160.SceneManagement
{
    [CreateAssetMenu(menuName = "d4160/SceneManagement/Scene Manager")]
    public class SceneManagerSO : ScriptableObject
    {
        [InspectInline(canEditRemoteTarget = true)]
        public ScenePackSO[] scenePacks;
    }
}