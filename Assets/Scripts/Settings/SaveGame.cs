#define USE_MYGAMES_FOLDER
#if STEAMWORKS_ENABLED
#endif
using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Demonixis.InMoov.Settings
{
    public enum GameSaveStorageMode
    {
        External = 0, Internal, Auto
    }

    [Serializable]
    public abstract class SaveGame
    {
        public static GameSaveStorageMode GetPreferredStorageMode()
        {
#if UNITY_STANDALONE || (UNITY_EDITOR && UNITY_STANDALONE)
            return GameSaveStorageMode.External;
#else
            return GameSaveStorageMode.Internal;
#endif
        }

        public static string GetAndroidPath(string additionalPath = null)
        {
            var path = $"/storage/emulated/0/Android/data/{Application.identifier}/files";

            if (!string.IsNullOrEmpty(additionalPath))
            {
                path = $"{path}/{additionalPath}";
            }

            return path;
        }

        public static string GetStandalonePath(string additionalPath = null)
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(root, Application.companyName, Application.productName);

#if USE_MYGAMES_FOLDER
            path = Path.Combine(root, "My Games", Application.productName);
#endif

            if (!string.IsNullOrEmpty(additionalPath))
            {
                path = Path.Combine(path, additionalPath);
            }

            return path;
        }

        private static string GetFallbackPath(string additionalPath = null)
        {
            return Application.persistentDataPath;
        }

        public static string GetSavePath(string additionalPath = null)
        {
            var path = GetFallbackPath(additionalPath);
#if UNITY_STANDALONE || UNITY_EDITOR
            path = GetStandalonePath(additionalPath);
#elif UNITY_ANDROID
            path = GetAndroidPath(additionalPath);
#endif

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }

            return path;
        }

        public static string SaveRawData(GameSaveStorageMode storageMode, object data, string filename, string additionalPath = null)
        {
            var json = data is string ? (string)data : JsonConvert.SerializeObject(data, Formatting.Indented);
            var externalStorageSave = storageMode != GameSaveStorageMode.Internal;

            if (externalStorageSave)
            {
#if UNITY_ANDROID
                externalStorageSave = Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite);
#elif UNITY_STANDALONE
                externalStorageSave = true;
#else
                externalStorageSave = false;
                Debug.Log("External Storage not supported on this platform. Switching to internal");
                storageMode = GameSaveStorageMode.Internal;
#endif
            }

            if (storageMode == GameSaveStorageMode.External && !externalStorageSave)
            {
                Debug.LogError($"Can't write {filename} to the external storage");
                return null;
            }

            if (externalStorageSave)
            {
                var savePath = Path.Combine(GetSavePath(additionalPath), filename);
                File.WriteAllText(savePath, json);
            }
            else
            {
                PlayerPrefs.SetString(filename, json);
                PlayerPrefs.Save();
            }
            return json;
        }

        public static T LoadRawData<T>(GameSaveStorageMode storageMode, string filename, string additionalPath = null)
        {
            var json = string.Empty;
            var externalStorageRead = storageMode != GameSaveStorageMode.Internal;

            if (externalStorageRead)
            {
#if UNITY_ANDROID
                externalStorageRead = Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);
#elif UNITY_STANDALONE
                externalStorageRead = true;
#else
                externalStorageRead = false;
                Debug.Log("External Storage not supported on this platform. Switching to internal");
                storageMode = GameSaveStorageMode.Internal;
#endif
            }

            if (storageMode == GameSaveStorageMode.External && !externalStorageRead)
            {
                Debug.LogError($"Can't read {filename} to the external storage");
                return default(T);
            }

            if (externalStorageRead)
            {
                var filepath = Path.Combine(GetSavePath(additionalPath), filename);

                if (File.Exists(filepath))
                {
                    json = File.ReadAllText(filepath);
                }
            }
            else
            {
                json = PlayerPrefs.GetString(filename, string.Empty);
            }

            if (!string.IsNullOrEmpty(json) && json != "{}")
            {
                var result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }

            return default(T);
        }

        public static string SaveData(GameSaveStorageMode storageMode, SaveGame saveGame, string filename, string additionalFolder = null)
        {
            return SaveRawData(storageMode, saveGame, filename, additionalFolder);
        }

        public static T LoadData<T>(GameSaveStorageMode storageMode, string filename, string additionalFolder = null) where T : SaveGame
        {
            return LoadRawData<T>(storageMode, filename, additionalFolder);
        }

        public static void SaveToCloud(object data, string filename, string additionalFolder = null)
        {
#if CLOUD_STORAGE_ENABLED
#if STEAMWORKS_ENABLED
            if (SteamManager.Available)
            {
                SteamManager.SaveToCloud(filename, JsonUtility.ToJson(data));
            }
#endif
#endif
        }

        public static T LoadFromCloud<T>(string filename)
        {
#if CLOUD_STORAGE_ENABLED
#if STEAMWORKS_ENABLED
            if (SteamManager.Available)
            {
                var json = SteamManager.LoadFromCloud(filename);

                if (!string.IsNullOrEmpty(json) && json != "{}")
                {
                    var result = JsonUtility.FromJson<T>(json);
                    return result;
                }
            }
#endif
#endif
            return default(T);
        }

        public static void ClearCloudData(string filename)
        {
#if CLOUD_STORAGE_ENABLED
#if STEAMWORKS_ENABLED
            if (SteamManager.Available)
            {
                SteamManager.SaveToCloud(filename, string.Empty);
            }
#endif
#endif
        }

        public static void ClearData(GameSaveStorageMode storageMode, string filename)
        {
            var externalStorageSave = storageMode != GameSaveStorageMode.Internal;

            if (externalStorageSave)
            {
#if UNITY_ANDROID
                externalStorageSave = Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite);
#elif UNITY_STANDALONE
                externalStorageSave = true;
#else
                externalStorageSave = false;
#endif
            }

            if (storageMode == GameSaveStorageMode.External && !externalStorageSave)
            {
                Debug.LogError($"Can't write {filename} to the external storage");
                return;
            }

            if (externalStorageSave)
            {
                var filepath = Path.Combine(GetSavePath(), filename);

                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
            }
            else if (PlayerPrefs.HasKey(filename))
            {
                PlayerPrefs.DeleteKey(filename);
            }
        }
    }
}