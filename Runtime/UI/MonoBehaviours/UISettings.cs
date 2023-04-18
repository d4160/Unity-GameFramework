using d4160.Core;
using d4160.Events;
using d4160.Persistence;
using d4160.Variables;
using UnityEngine;

namespace d4160.UIs
{
    public abstract class UISettings<T> : MonoBehaviour
    {
        [SerializeField] protected UnityLifetimeMethodType _loadSettingsAt;

        [Header("Data")]
        [SerializeField] protected VariableSOBase<T> _var;
        [SerializeField] protected PlayerPrefsSO _prefs;

        protected EventSOBase<T>.EventListener _onValueChanged;

        protected virtual void Awake()
        {
            _onValueChanged = new(SetSettings);

            if (_loadSettingsAt == UnityLifetimeMethodType.Awake)
            {
                LoadSettings();
            }
        }

        protected virtual void Start()
        {
            if (_loadSettingsAt == UnityLifetimeMethodType.Start)
            {
                LoadSettings();
            }
        }

        protected virtual void OnEnable()
        {
            if (_loadSettingsAt == UnityLifetimeMethodType.OnEnable)
            {
                LoadSettings();
            }

            _var.OnValueChange.AddListener(_onValueChanged);
        }

        protected virtual void OnDisable()
        {
            _var.OnValueChange.RemoveListener(_onValueChanged);
        }

        protected virtual void LoadSettings()
        {
            _prefs.Load();
            SetSettings(_var.Value);
        }

        protected abstract void SetSettings(T value);
    }
}