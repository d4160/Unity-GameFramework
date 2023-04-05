using UnityEngine;
using d4160.Collections;
using d4160.Variables;
using d4160.Events;
using UMA.CharacterSystem;
using UMA;
using d4160.Singleton;
using NaughtyAttributes;

namespace d4160.UMA
{
    public class DCACustomizer : Singleton<DCACustomizer>, IEventListener<int>
    {
        [Header("References")]
        [SerializeField] private DynamicCharacterAvatar _staticDCA;

        [Header("Options"), Tooltip("When is true, inmediate changes are only applied for the static DCA, so you need to manually call LoadRecipe(false) to apply changes for Local Player Avatar.")]
        [SerializeField] private bool _applyChangesOnlyForStaticDCA;

        [Header("DataLibraries")]
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private ObjectLibrarySO _umaRacesLib;

        [Header("Variables")]
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _umaRecipeVar;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private GameObjectVariableSO _localPlayerVar;

        [Header("Events")]
        [SerializeField] private IntEventSO _onGenreChanged;

        private bool ApplyChangesForLocalAvatar => Avatar && (!_staticDCA || !_applyChangesOnlyForStaticDCA);

        private DynamicCharacterAvatar _avatar;
        private DynamicCharacterAvatar Avatar
        {
            get
            {
                if (!_avatar && _localPlayerVar.Value)
                {
                    _avatar = _localPlayerVar.Value.GetComponentInChildren<DynamicCharacterAvatar>();
                }
                return _avatar;
            }
        }

        private void OnEnable()
        {
            _onGenreChanged.AddListener(this);
        }

        private void Start()
        {
            LoadRecipe();
        }

        private void OnDisable()
        {
            _onGenreChanged.RemoveListener(this);
        }

        void IEventListener<int>.OnInvoked(int newGenre)
        {
            if (ApplyChangesForLocalAvatar)
            {
                Avatar.ChangeRace(_umaRacesLib.GetAs<RaceData>(newGenre));
            }

            if (_staticDCA)
            {
                _staticDCA.ChangeRace(_umaRacesLib.GetAs<RaceData>(newGenre));
            }
        }

        public void SaveRecipe()
        {
            if (_staticDCA)
            {
                _umaRecipeVar.Value = _staticDCA.GetCurrentRecipe();
            }
            else if (Avatar)
            {
                _umaRecipeVar.Value = Avatar.GetCurrentRecipe();
            }
        }

        public void LoadRecipe(bool forLocalAvatar = true)
        {
            if (!string.IsNullOrEmpty(_umaRecipeVar))
            {
                if (forLocalAvatar && Avatar)
                    Avatar.LoadFromRecipeString(_umaRecipeVar);
                if (!forLocalAvatar && _staticDCA)
                    _staticDCA.LoadFromRecipeString(_umaRecipeVar);
            }
        }
    }
}