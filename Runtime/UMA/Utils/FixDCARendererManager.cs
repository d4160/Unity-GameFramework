using System.Collections;
using UnityEngine;
using UMA.CharacterSystem;
using d4160.MonoBehaviours;

namespace d4160.UMA
{
    [RequireComponent(typeof(DCARendererManager))]
    public class FixDCARendererManager : DelayedStartBehaviourBase
    {
        [SerializeField] private bool _renderersEnabled;

        private DCARendererManager _renMan;

        private void Awake()
        {
            _renMan = GetComponent<DCARendererManager>();
        }

#if UNITY_EDITOR
        private void Reset()
        {
            GetComponent<DCARendererManager>().RenderersEnabled = false;
        }
#endif

        protected override void OnStart()
        {
            _renMan.RenderersEnabled = _renderersEnabled;
        }
    }
}