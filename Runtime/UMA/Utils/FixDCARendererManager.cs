using System.Collections;
using UnityEngine;
using UMA.CharacterSystem;
using d4160.MonoBehaviours;

namespace d4160.UMA
{
    [RequireComponent(typeof(DCARendererManager), typeof(DynamicCharacterAvatar))]
    public class FixDCARendererManager : DelayedStartBehaviourBase
    {
        [SerializeField] private bool _renderersEnabled;

        private DCARendererManager _renMan;
        private DynamicCharacterAvatar _dca;

        private void Awake()
        {
            _renMan = GetComponent<DCARendererManager>();
            _dca = GetComponent<DynamicCharacterAvatar>();
        }

#if UNITY_EDITOR
        private void Reset()
        {
            GetComponent<DCARendererManager>().RenderersEnabled = false;
        }
#endif

        protected override void OnStart()
        {
            if (_dca.activeRace.racedata != null)
                _renMan.RenderersEnabled = _renderersEnabled;
        }
    }
}