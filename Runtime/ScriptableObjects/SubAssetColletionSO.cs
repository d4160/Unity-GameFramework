using UnityEngine;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using System.Collections.Generic;
#if UNITY_EDITOR
using d4160.Core.Editors.Utilities;
using d4160.ScriptableObjects.Editors.Utilities;
#endif
using d4160.Collections;
using Malee;
using InspectInLine;

namespace d4160.ScriptableObjects
{
    public abstract class SubAssetCollectionSO<T> : ScriptableObject
        , IScriptableCopy<T>
#if ENABLE_NAUGHTY_ATTRIBUTES
    , IReorderableObjectOnRemoved
#endif
        where T : ScriptableObject

    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Reorderable(paginate = true)]
#endif
        [SerializeField] protected SubAssetArray _array;

        [Header("OPTIONS")]
        [SerializeField] protected bool _showOptions;
        [SerializeField]
#if ENABLE_NAUGHTY_ATTRIBUTES
        [ShowIf("_showOptions")]
#endif
        protected int _selectedIndex;
        [SerializeField, InspectInline(canEditRemoteTarget = true)]
#if ENABLE_NAUGHTY_ATTRIBUTES
        [ShowIf("_showOptions")]
#endif
        protected SubAssetsImportArraySO _importArraySO;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [ShowIf("_showOptions")]
#endif
        [SerializeField, InspectInline(canEditRemoteTarget = true)] protected SubAssetsExportArraySO _exportArraySO;

        public int Count => _array.Count;

        #region EDITOR
#if UNITY_EDITOR

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button, ShowIf("_showOptions")]
#endif

        public virtual void Add()
        {
            var subasset = CreateInstance<T>();
#if ENABLE_NAUGHTY_ATTRIBUTES
            subasset.name = $"{_array.Count}";
            this.AddSubasset(subasset);
            _array.Add(subasset);
#endif
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button, ShowIf("_showOptions")]
#endif
        public virtual void DuplicateSelectedIndex()
        {
            if (_array.IsValidIndex(_selectedIndex) && _array[_selectedIndex])
            {
                T copy = this.CreateCopy(_array[_selectedIndex]);
                copy.name = $"{_selectedIndex} (copy)";
                this.AddSubasset(copy);
                _array.Insert(_selectedIndex + 1, copy);
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button, ShowIf("_showOptions")]
#endif
        public virtual void DestroySelectedIndex()
        {
            if (_array.IsValidIndex(_selectedIndex) && _array[_selectedIndex])
            {
                this.RemoveSubAsset(_array[_selectedIndex]);
                _array.RemoveAt(_selectedIndex);
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button, ShowIf("_showOptions")]
#endif
        public virtual void DestroyAll()
        {
            for (var i = _array.Count - 1; i >= 0; i--)
            {
                this.RemoveSubAsset(_array[i]);
                _array.RemoveAt(i);
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button, ShowIf("_showOptions")]
#endif
        public void SyncNames()
        {
#if ENABLE_NAUGHTY_ATTRIBUTES
            for (var i = 0; i < _array.Count; i++)
            {
                _array[i].name = GetName(i);
            }
#endif

            this.Reimport();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button, ShowIf("_showOptions")]
#endif
        public void SubObjectsToArrayAndReplace()
        {
            _array.Clear();

            var objects = this.GetSubObjectsOfType<T>();
            objects.ForEach((x) => {
                _array.Add(x);
            });
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button, ShowIf("_showOptions")]
#endif
        public void SubObjectsToExportArray()
        {
            var indexes = new List<int>();
            for (int i = 0; i < _array.Count; i++)
            {
                indexes.Add(i);
            }

            if (_exportArraySO)
            {
                _exportArraySO.Items = indexes;
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button, ShowIf("_showOptions")]
#endif
        public void ImportAndReplaceSoft()
        {
            if (_importArraySO)
            {
                _importArraySO.Items.ForEach(x => {
                    if (x.sObject is T tObject)
                    {
                        if (_array.IsValidIndex(x.index))
                        {
                            _array[x.index] = tObject;
                        }
                        else
                        {
                            if (x.index >= _array.Count)
                            {
                                for (int i = _array.Count; i <= x.index; i++)
                                {
                                    _array.Add(null);
                                }
                                _array[x.index] = tObject;
                            }
                        }
                    }
                });
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button, ShowIf("_showOptions")]
#endif
        public void ImportAndReplaceHard()
        {
            if (_importArraySO)
            {
                _importArraySO.Items.ForEach(x => {
                    if (x.sObject is T tObject)
                    {
                        if (_array.Contains(tObject))
                            return;

                        if (_array.IsValidIndex(x.index))
                        {
                            if (_array[x.index])
                            {
                                this.RemoveSubAsset(_array[x.index], false);
                            }

                            var newInstance = this.CreateCopy(tObject);
                            this.AddSubasset(newInstance, false);

                            _array[x.index] = newInstance;
                        }
                        else
                        {
                            if (x.index >= _array.Count)
                            {
                                for (int i = _array.Count; i <= x.index; i++)
                                {
                                    _array.Add(null);
                                }

                                var newInstance = this.CreateCopy(tObject);
                                this.AddSubasset(newInstance, false);

                                _array[x.index] = newInstance;
                            }
                        }
                    }
                });

                SyncNames();

                this.Reimport();
            }
        }



#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button, ShowIf("_showOptions")]
#endif
        public void ExportSubAssets()
        {
            string path = UnityEditor.EditorUtility.OpenFolderPanel("Save Folder", "", "");
            if (string.IsNullOrEmpty(path)) return;
            
            var relativePath = $"{EditorUtility.AssetsRelativePath(path)}";
            var subObjects = this.GetSubObjectsOfType<T>();

            var importArraySO = ScriptableObject.CreateInstance<SubAssetsImportArraySO>();
            var importItems = new List<ImportItem>();
            var index = 0;
            _exportArraySO.Items.ForEach(x => {
                if (subObjects.IsValidIndex(x))
                {
                    var obj = subObjects[x];
                    if (obj)
                    {
                        var newInstance = this.CreateCopy(obj);
                        var assetName = obj.name.Replace(":", "").Replace("\\", "");

                        UnityEditor.AssetDatabase.CreateAsset(newInstance, $"{relativePath}/{assetName}_{new System.Guid()}.asset");

                        importItems.Add(new ImportItem() { sObject = newInstance, index = index });
                    }
                }

                index++;
            });

            importArraySO.Items = importItems;
            UnityEditor.AssetDatabase.CreateAsset(importArraySO, $"{relativePath}/SubAssets ImportArray_{new System.Guid()}.asset");
            if (!_importArraySO) _importArraySO = importArraySO;

            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.ImportAsset(path);
        }
#endif
        #endregion

        public T this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }

        protected abstract string GetName(int i);
        public abstract void Copy(T copyFrom, T refCopyTo);

#if ENABLE_NAUGHTY_ATTRIBUTES
        void IReorderableObjectOnRemoved.OnRemoved(int index)
        {
            //if(numbers[index])
            //{
            //    this.RemoveSubAsset(numbers[index]);
            //}
        }
#endif

        [System.Serializable]
        public class SubAssetArray
#if ENABLE_NAUGHTY_ATTRIBUTES
            : ReorderableArrayForUnityObject<T>
#else
            : List<T>
#endif
        {

        }
    }
}

