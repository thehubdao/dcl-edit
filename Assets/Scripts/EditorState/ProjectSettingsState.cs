using System.IO;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zenject;

namespace Assets.Scripts.EditorState
{
    public class ProjectSettingsState
    {
        // Dependencies
        private PathState _pathState;

        [Inject]
        private void Constructor(PathState pathState)
        {
            _pathState = pathState;
        }


        private JObject _projectSettings = null;

        public MyNullable<T> GetSetting<T>(string key)
        {
            if (_projectSettings == null)
            {
                LoadSettings();
            }

            return _projectSettings!.ContainsKey(key) ?
                _projectSettings[key]!.ToObject<T>() :
                new MyNullable<T>();
        }

        public void SetSetting<T>(string key, T value)
        {
            if (_projectSettings == null)
            {
                LoadSettings();
            }

            _projectSettings![key] = JToken.FromObject(value);

            SaveSettings();
        }

        private void SaveSettings()
        {
            var projectSettingsDirectoryPath = _pathState.ProjectPath + "/dcl-edit";
            var projectSettingsPath = projectSettingsDirectoryPath + "/settings.json";

            if (!Directory.Exists(projectSettingsDirectoryPath))
            {
                Directory.CreateDirectory(projectSettingsDirectoryPath);
            }

            File.WriteAllText(projectSettingsPath, JsonConvert.SerializeObject(_projectSettings, Formatting.Indented));
        }

        private void LoadSettings()
        {
            var projectSettingsPath = _pathState.ProjectPath + "/dcl-edit/settings.json";

            _projectSettings = File.Exists(projectSettingsPath) ?
                JObject.Parse(File.ReadAllText(projectSettingsPath)) :
                new JObject();
        }
    }
}
