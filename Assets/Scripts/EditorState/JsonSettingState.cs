using System;
using System.IO;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.EditorState
{
    public abstract class JsonSettingState
    {
        // Dependencies
        protected PathState _pathState;
        protected SceneManagerState sceneManagerState;

        [Inject]
        private void Constructor(PathState pathState, SceneManagerState sceneManagerState)
        {
            _pathState = pathState;
            this.sceneManagerState = sceneManagerState;
        }


        protected JObject _jsonSettings = null;

        public MyNullable<T> GetSetting<T>(string key)
        {
            RequireSettingsLoaded();

            return _jsonSettings!.ContainsKey(key) ?
                _jsonSettings[key]!.ToObject<T>() :
                new MyNullable<T>();
        }

        public void SetSetting<T>(string key, T value)
        {
            RequireSettingsLoaded();

            _jsonSettings![key] = JToken.FromObject(value);

            SaveSettings();
        }

        private void RequireSettingsLoaded()
        {
            if (_jsonSettings == null)
            {
                LoadSettings();
            }
        }

        protected abstract void SaveSettings();
        protected abstract void LoadSettings();
    }

    public class ProjectSettingState : JsonSettingState
    {
        private string _projectSettingDirectoryPath => _pathState.ProjectPath + "/dcl-edit/";

        private string _projectSettingPath => _projectSettingDirectoryPath + "settings.json";

        protected override void SaveSettings()
        {
            if (!Directory.Exists(_projectSettingDirectoryPath))
            {
                Directory.CreateDirectory(_projectSettingDirectoryPath);
            }

            try
            {
                File.WriteAllText(_projectSettingPath, JsonConvert.SerializeObject(_jsonSettings, Formatting.Indented));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        protected override void LoadSettings()
        {
            _jsonSettings = File.Exists(_projectSettingPath) ?
                JObject.Parse(File.ReadAllText(_projectSettingPath)) :
                new JObject();
        }
    }

    public class SceneSettingState : JsonSettingState
    {
        protected override void SaveSettings()
        {
            if (!sceneManagerState.GetCurrentDirectoryState()?.IsSceneOpened() ?? true)
            {
                // when there is no scene, just return
                return;
            }

            var sceneJsonPath = sceneManagerState.GetCurrentDirectoryState()!.directoryPath + "/scene.json";

            JObject sceneJson;
            try
            {
                sceneJson = File.Exists(sceneJsonPath) ?
                    JObject.Parse(File.ReadAllText(sceneJsonPath)) :
                    new JObject();
            }
            catch (Exception)
            {
                sceneJson = new JObject();
            }

            sceneJson["settings"] = _jsonSettings;
            try
            {
                File.WriteAllText(sceneJsonPath, JsonConvert.SerializeObject(sceneJson, Formatting.Indented));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        protected override void LoadSettings()
        {
            JObject sceneJson;
            try
            {
                var currentDirectoryState = sceneManagerState.GetCurrentDirectoryState();

                if (currentDirectoryState == null)
                {
                    throw new Exception();
                }

                var sceneJsonPath = currentDirectoryState.directoryPath + "/scene.json";

                sceneJson = File.Exists(sceneJsonPath) ?
                    JObject.Parse(File.ReadAllText(sceneJsonPath)) :
                    new JObject();
            }
            catch (Exception)
            {
                sceneJson = new JObject();
            }

            _jsonSettings = sceneJson.SelectToken("settings") as JObject ?? new JObject();
        }
    }
}
