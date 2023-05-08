using UnityEngine;
using d4160.Collections;
using d4160.Variables;
using d4160.Events;
using UMA.CharacterSystem;
using UMA;
using d4160.Singleton;
using NaughtyAttributes;
using System.Collections.Generic;
using System;

namespace d4160.UMA
{
    public class DCACustomizer : Singleton<DCACustomizer>
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
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private UmaRefsGroupLibrarySO _dnaLib;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private UmaRefsGroupLibrarySO _colorLib;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringLibrarySO _slotLib;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private DualObjectLibrarySO _hairDualLib;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private ObjectLibrarySO _beardLib;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private DualObjectLibrarySO _overClothingDualLib;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private DualObjectLibrarySO _chestDualLib;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private DualObjectLibrarySO _legsDualLib;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private DualObjectLibrarySO _feetDualLib;

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

        private IntEventSO.EventListener _onGenreChangedLtn;
        private GameObjectEventSO.EventListener _onLocalPlayerChangedLtn;

        private Dictionary<string, DnaSetter> _localAvatarDna;
        private Dictionary<string, DnaSetter> _staticAvatarDna;

        private DynamicCharacterAvatar _oldLocalAvatar;

        private bool ApplyChangesForLocalAvatar => Avatar && (!_staticDCA || !_applyChangesOnlyForStaticDCA);

        private DynamicCharacterAvatar _avatar;
        private DynamicCharacterAvatar Avatar
        {
            get
            {
                if (!_avatar && _localPlayerVar.Value)
                {
                    _avatar = _localPlayerVar.Value.GetComponentInChildren<DynamicCharacterAvatar>();
                    _oldLocalAvatar = _avatar;
                }
                return _avatar;
            }
        }

        public DynamicCharacterAvatar StaticDCA { 
            get => _staticDCA; 
            set
            {
                if (_staticDCA != value)
                {
                    if (_staticDCA != null) _staticDCA.CharacterUpdated.RemoveListener(StaticAvatar_CharacterUpdated);

                    _staticDCA = value;
                    if (value)
                    {
                        try
                        {
                            _staticAvatarDna = _staticDCA.GetDNA();
                        }
                        catch
                        {
                            _staticAvatarDna = null;
                        }
                        _staticDCA.CharacterUpdated.AddListener(StaticAvatar_CharacterUpdated);
                    }
                }
            } 
        }

        [ContextMenu("PrintStaticAvatarDNA")]
        private void PrintStaticAvatarDNA()
        {
            PrintAvatarDNA(_staticAvatarDna);
        }

        [ContextMenu("PrintLocalAvatarDNA")]
        private void PrintLocalAvatarDNA()
        {
            PrintAvatarDNA(_localAvatarDna);
        }

        private void PrintAvatarDNA(Dictionary<string, DnaSetter> dna)
        {
            if (dna != null)
            {
                string s = string.Empty;
                foreach (var item in dna)
                {
                    s += $"{item.Key},";
                }

                Debug.Log(s);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _onGenreChangedLtn = new((genre) => {
                if (ApplyChangesForLocalAvatar)
                {
                    Avatar.ChangeRace(_umaRacesLib.GetAs<RaceData>(genre));
                }

                if (_staticDCA)
                {
                    _staticDCA.ChangeRace(_umaRacesLib.GetAs<RaceData>(genre));
                }
            });

            _onLocalPlayerChangedLtn = new((go) => {

                if (_oldLocalAvatar) _oldLocalAvatar.CharacterUpdated.RemoveListener(LocalAvatar_CharacterUpdated);
                if (go)
                {
                    //_localAvatarDna = Avatar.GetDNA();
                    Avatar.CharacterUpdated.AddListener(LocalAvatar_CharacterUpdated);
                }
            });
        }

        private void OnEnable()
        {
            _onGenreChanged.AddListener(_onGenreChangedLtn);
            _localPlayerVar.OnValueChange.AddListener(_onLocalPlayerChangedLtn);
        }

        private void Start()
        {
            LoadRecipe();

            if (_localPlayerVar.Value)
            {
                //_localAvatarDna = Avatar.GetDNA();
                Avatar.CharacterUpdated.AddListener(LocalAvatar_CharacterUpdated);
            }

            if (_staticDCA)
            {
                //_staticAvatarDna = _staticDCA.GetDNA();
                _staticDCA.CharacterUpdated.AddListener(StaticAvatar_CharacterUpdated);
            }
        }

        private void OnDisable()
        {
            _onGenreChanged.RemoveListener(_onGenreChangedLtn);
            _localPlayerVar.OnValueChange.RemoveListener(_onLocalPlayerChangedLtn);
        }

        private void OnDestroy()
        {
            if (Avatar)
                Avatar.CharacterUpdated.RemoveListener(LocalAvatar_CharacterUpdated);

            if (_staticDCA)
                _staticDCA.CharacterUpdated.RemoveListener(LocalAvatar_CharacterUpdated);
        }

        private void LocalAvatar_CharacterUpdated(UMAData data)
        {
            _localAvatarDna = Avatar.GetDNA();
        }

        private void StaticAvatar_CharacterUpdated(UMAData data)
        {
            _staticAvatarDna = _staticDCA.GetDNA();
        }

        public void SetDNA(int index, float value)
        {
            _dnaLib.SetDNA(index, ApplyChangesForLocalAvatar ? (_localAvatarDna ??= Avatar.GetDNA()) : (_staticAvatarDna ??= StaticDCA.GetDNA()), value, ApplyChangesForLocalAvatar ? Avatar : StaticDCA);
        }

        public float GetDNA(int index)
        {
            try {
                return _dnaLib.GetDNA(index, ApplyChangesForLocalAvatar ? (_localAvatarDna ??= Avatar.GetDNA()) : (_staticAvatarDna ??= StaticDCA.GetDNA()));
            }
            catch
            {
                return 0.5f;
            }
        }

        public void SetColor(int index, Color color)
        {
            _colorLib.SetColor(index, ApplyChangesForLocalAvatar ? Avatar : StaticDCA, color);
        }

        public Color GetColor(int index)
        {
            return _colorLib.GetColor(index, ApplyChangesForLocalAvatar ? Avatar : StaticDCA);
        }

        public void SetHairSlot(int slotIdx, int index, int genre)
        {
            SetSlot(_hairDualLib, slotIdx, index, genre);
        }

        // Only for genre male
        public void SetBeardSlot(int slotIdx, int index)
        {
            SetSlot(_beardLib, slotIdx, index);
        }

        public void SetOverClothingSlot(int slotIdx, int index, int genre)
        {
            SetSlot(_overClothingDualLib, slotIdx, index, genre);
        }

        public void SetChestSlot(int slotIdx, int index, int genre)
        {
            SetSlot(_chestDualLib, slotIdx, index, genre);
        }

        public void SetLegsSlot(int slotIdx, int index, int genre)
        {
            SetSlot(_legsDualLib, slotIdx, index, genre);
        }

        public void SetFeetSlot(int slotIdx, int index, int genre)
        {
            SetSlot(_feetDualLib, slotIdx, index, genre);
        }

        private void SetSlot(DualObjectLibrarySO dualLib, int slotIdx, int index, int genre)
        {
            if (!_slotLib.Items.IsValidIndex(slotIdx)) return;

            var recipe = dualLib.GetAs<UMAWardrobeRecipe>(index, genre);

            DynamicCharacterAvatar avatar = ApplyChangesForLocalAvatar ? Avatar : StaticDCA;

            if (recipe)
                avatar.SetSlot(recipe);
            else
                avatar.ClearSlot(_slotLib[slotIdx]);

            avatar.BuildCharacter();
        }

        private void SetSlot(ObjectLibrarySO lib, int slotIdx, int index)
        {
            if (!_slotLib.Items.IsValidIndex(slotIdx)) return;

            var recipe = lib.GetAs<UMAWardrobeRecipe>(index);

            DynamicCharacterAvatar avatar = ApplyChangesForLocalAvatar ? Avatar : StaticDCA;

            if (recipe)
                avatar.SetSlot(recipe);
            else
                avatar.ClearSlot(_slotLib[slotIdx]);

            avatar.BuildCharacter();
        }

        public T GetSlotOrDefault<T>(int slotIdx, Func<UMATextRecipe, T> returnVal, T defaultVal)
        {
            UMATextRecipe recipe = GetSlot(slotIdx);
            if (recipe == null) return defaultVal;
            else return returnVal(recipe);
        }

        public UMATextRecipe GetSlot(int slotIdx)
        {
            if (!_slotLib.Items.IsValidIndex(slotIdx)) return default;

            DynamicCharacterAvatar avatar = ApplyChangesForLocalAvatar ? Avatar : StaticDCA;
            try
            {
                UMATextRecipe recipe = avatar.GetWardrobeItem(_slotLib[slotIdx]);
                return recipe;
            }
            catch
            {
                return default;
            }
        }

        public void SaveRecipe()
        {
            if (_staticDCA)
            {
                Debug.Log("[SaveRecipe] StaticDCA");
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
                {
                    Debug.Log($"[LoadRecipe] ForLocalAvatar");
                    Avatar.UnloadAllWardrobeCollections();
                    Avatar.LoadFromRecipeString(_umaRecipeVar, DynamicCharacterAvatar.LoadOptions.useDefaults, true);
                }
                if (!forLocalAvatar && _staticDCA)
                {
                    _staticDCA.UnloadAllWardrobeCollections();
                    _staticDCA.LoadFromRecipeString(_umaRecipeVar);
                }
            }
        }
    }
}