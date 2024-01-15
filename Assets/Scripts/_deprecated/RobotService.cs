using Demonixis.InMoov.Settings;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Demonixis.InMoov
{
    /// <summary>
    /// Base skeleton of a robot service.
    /// A service must have a type. By default it is supported on all platforms.
    /// </summary>
    public abstract class RobotService : MonoBehaviour
    {
        public virtual RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.Android,
            RuntimePlatform.LinuxEditor,
            RuntimePlatform.LinuxPlayer,
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer,
            RuntimePlatform.OSXEditor,
            RuntimePlatform.OSXPlayer
        };

        protected Dictionary<string, string> _customSettings;

        public virtual string SerializationFilename { get; }

        public string ServiceName => GetType().Name;
        public bool Started { get; protected set; }
        public bool Paused { get; protected set; }

        protected virtual void Awake()
        {
            if (Array.IndexOf(SupportedPlateforms, Application.platform) == -1)
                Destroy(this);
        }

        public virtual void Initialize()
        {
            if (!string.IsNullOrEmpty(SerializationFilename))
                _customSettings = SaveGame.LoadRawData<Dictionary<string, string>>(SaveGame.GetPreferredStorageMode(), SerializationFilename, "Config");

            _customSettings ??= new Dictionary<string, string>();

            Started = true;
        }

        public virtual void SetPaused(bool paused)
        {
            Paused = paused;
        }

        public virtual void Shutdown()
        {
            if (!string.IsNullOrEmpty(SerializationFilename))
                SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), _customSettings, SerializationFilename, "Config");

            Started = false;
        }

        public bool IsSupported()
        {
            return Array.IndexOf(SupportedPlateforms, Application.platform) > -1;
        }

        protected void AddSetting(string key, string value)
        {
            if (!_customSettings.ContainsKey(key))
                _customSettings.Add(key, value);
            else
                _customSettings[key] = value;
        }

        protected void RemoveSetting(string key)
        {
            if (_customSettings.ContainsKey(key))
                _customSettings.Remove(key);
        }
    }
}