using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using d4160.GameFramework.Authentication;
using Ludiq;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.GameFoundation.Data;
using UnityEngine.GameFoundation.DefaultCatalog;
using UnityEngine.GameFoundation.DefaultLayers.Persistence;

namespace d4160.GameFramework.DataPersistence
{
    /// <summary>
    /// This saves data locally onto the user's device.
    /// </summary>
    public class PlayFabPersistence : BaseDataPersistence
    {
        /// <summary>
        /// Suffix used for the backup save file.
        /// </summary>
        public const string kBackupSuffix = "_backup";

        public string Key { get; }

        protected string _playFabId;

        /// <summary>
        /// Initializes the local persistence with the given file path & serializer.
        /// </summary>
        /// <param name="key">
        /// The relative path from <see cref="Application.persistentDataPath"/> to the save file.
        /// </param>
        /// <param name="serializer">The data serializer to use.</param>
        public PlayFabPersistence(string key, IDataSerializer serializer)
            : base(serializer)
        {
            Key = key;
            _playFabId = PlayFabAuthService.Instance.Id;
        }

        /// <inheritdoc />
        public override void Save(
            GameFoundationData content,
            Action onSaveCompleted = null,
            Action<Exception> onSaveFailed = null)
        {
            string keyBackup = $"{Key}{kBackupSuffix}";

            try
            {
                string serializedData = serializer.Serialize(content);

                PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
                    {
                        Permission = UserDataPermission.Private,
                        Data = new Dictionary<string, string>()
                        {
                            { Key,  serializedData },
                            { keyBackup, serializedData }
                        }
                    }, 
                    (result) => { onSaveCompleted?.Invoke(); },
                    (error) => { onSaveFailed?.Invoke(new Exception(error.ErrorMessage)); });
            }
            catch (Exception e)
            {
                onSaveFailed?.Invoke(e);
            }
        }

        /// <inheritdoc />
        public override void Load(
            Action<GameFoundationData> onLoadCompleted = null,
            Action<Exception> onLoadFailed = null)
        {
            string keyBackup = $"{Key}{kBackupSuffix}";

            try
            {
                PlayFabClientAPI.GetUserData(new GetUserDataRequest()
                {
                    PlayFabId = _playFabId,
                    Keys = new List<string> { Key, keyBackup }
                }, (result) =>
                {
                    if (result.Data.ContainsKey(Key))
                    {
                        GameFoundationData data = serializer.Deserialize(result.Data[Key].Value);
                        onLoadCompleted?.Invoke(data);
                    }
                    else if(result.Data.ContainsKey(keyBackup))
                    {
                        GameFoundationData data = serializer.Deserialize(result.Data[keyBackup].Value);
                        onLoadCompleted?.Invoke(data);
                    }
                    else
                    {
                        Debug.LogWarning($"The PlayerData (Title) with key '{Key}' (also backup) is missing in player: '{_playFabId}'. So GameFoundation has default data from Database.");
                        onLoadCompleted?.Invoke(GameFoundationDatabaseSettings.database.CreateDefaultData());
                    }
                }, (error) =>
                {
                    onLoadFailed?.Invoke(new Exception(error.ErrorMessage));
                });
            }
            catch (Exception e)
            {
                onLoadFailed?.Invoke(e);
            }
        }

        /// <summary>
        /// Asynchronously delete data from the persistence layer.
        /// </summary>
        /// <param name="onDeletionCompleted">Called when the deletion is completed with success.</param>
        /// <param name="onDeletionFailed">Called with a detailed exception when the deletion failed.</param>
        public void Delete(
            Action onDeletionCompleted = null,
            Action<Exception> onDeletionFailed = null)
        {
            string keyBackup = $"{Key}{kBackupSuffix}";

            try
            {
                PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
                    {
                        KeysToRemove = new List<string> { Key, keyBackup }
                    }, 
                    (result) => { onDeletionCompleted?.Invoke(); },
                    (error) => { onDeletionFailed?.Invoke(new Exception(error.ErrorMessage)); });
            }
            catch (Exception e)
            {
                onDeletionFailed?.Invoke(e);
            }
        }
    }
}
