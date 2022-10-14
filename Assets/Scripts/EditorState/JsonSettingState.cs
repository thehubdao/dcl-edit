using System;
using System.IO;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zenject;

namespace Assets.Scripts.EditorState
{
    public abstract class JsonSettingState
    {
        // Dependencies
        protected PathState _pathState;
        protected SceneDirectoryState _sceneDirectoryState;

        [Inject]
        private void Constructor(PathState pathState, SceneDirectoryState sceneDirectoryState)
        {
            _pathState = pathState;
            _sceneDirectoryState = sceneDirectoryState;
        }


        protected JObject _jsonSettings = null;

        public MyNullable<T> GetSetting<T>(string key)
        {
            if (_jsonSettings == null)
            {
                LoadSettings();
            }

            return _jsonSettings!.ContainsKey(key) ?
                _jsonSettings[key]!.ToObject<T>() :
                new MyNullable<T>();
        }

        public void SetSetting<T>(string key, T value)
        {
            if (_jsonSettings == null)
            {
                LoadSettings();
            }

            _jsonSettings![key] = JToken.FromObject(value);

            SaveSettings();
        }

        protected abstract void SaveSettings();
        protected abstract void LoadSettings();
    }

    public class ProjectSettingState : JsonSettingState
    {
        protected override void SaveSettings()
        {
            var projectSettingsDirectoryPath = _pathState.ProjectPath + "/dcl-edit";
            var projectSettingsPath = projectSettingsDirectoryPath + "/settings.json";

            if (!Directory.Exists(projectSettingsDirectoryPath))
            {
                Directory.CreateDirectory(projectSettingsDirectoryPath);
            }

            File.WriteAllText(projectSettingsPath, JsonConvert.SerializeObject(_jsonSettings, Formatting.Indented));
        }

        protected override void LoadSettings()
        {
            var projectSettingsPath = _pathState.ProjectPath + "/dcl-edit/settings.json";

            _jsonSettings = File.Exists(projectSettingsPath) ?
                JObject.Parse(File.ReadAllText(projectSettingsPath)) :
                new JObject();
        }
    }

    public class SceneSettingState : JsonSettingState
    {
        protected override void SaveSettings()
        {
            if (!_sceneDirectoryState.IsSceneOpened())
            {
                return;
            }

            var sceneJsonPath = _sceneDirectoryState.DirectoryPath + "/scene.json";

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

            File.WriteAllText(sceneJsonPath, JsonConvert.SerializeObject(sceneJson, Formatting.Indented));
        }

        protected override void LoadSettings()
        {
            var sceneJsonPath = _sceneDirectoryState.DirectoryPath + "/scene.json";

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

            _jsonSettings = sceneJson.SelectToken("settings") as JObject ?? new JObject();
        }
    }
}
