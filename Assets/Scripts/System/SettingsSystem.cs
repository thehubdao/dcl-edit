using System.Collections.Generic;
using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class SettingsSystem
    {
        public enum SettingType
        {
            Text,
            Number,
            Integer,
            Vector3
        }

        public enum SettingStage
        {
            User,
            Project,
            Scene
        }

        public interface ISetting
        {
            string name { get; }

            string valueString { get; }

            SettingType type { get; }

            SettingStage stage { get; }
        }

        public abstract class UserSetting<T> : ISetting
        {
            protected UserSetting(string name, T defaultValue)
            {
                this.name = name;
                this.defaultValue = defaultValue;

                stage = SettingStage.User;
            }

            public string name { get; }
            public string valueString => Get().ToString();
            public T defaultValue { get; }
            public SettingType type { get; protected set; }
            public SettingStage stage { get; }
            public abstract T Get();
            public abstract void Set(T value);
        }

        public class StringUserSetting : UserSetting<string>
        {
            public StringUserSetting(string name, string defaultValue) : base(name, defaultValue)
            {
                type = SettingType.Text;
            }

            public override string Get()
            {
                return PlayerPrefs.HasKey(name) ?
                    PlayerPrefs.GetString(name) :
                    defaultValue;
            }

            public override void Set(string value)
            {
                PlayerPrefs.SetString(name, value);
            }
        }

        public class IntUserSetting : UserSetting<int>
        {
            public IntUserSetting(string name, int defaultValue) : base(name, defaultValue)
            {
                type = SettingType.Integer;
            }

            public override int Get()
            {
                return PlayerPrefs.HasKey(name) ?
                    PlayerPrefs.GetInt(name) :
                    defaultValue;
            }

            public override void Set(int value)
            {
                PlayerPrefs.SetInt(name, value);
            }
        }

        public class FloatUserSetting : UserSetting<float>
        {
            public FloatUserSetting(string name, float defaultValue) : base(name, defaultValue)
            {
                type = SettingType.Number;
            }

            public override float Get()
            {
                return PlayerPrefs.HasKey(name) ?
                    PlayerPrefs.GetFloat(name) :
                    defaultValue;
            }

            public override void Set(float value)
            {
                PlayerPrefs.SetFloat(name, value);
            }
        }

        public abstract class ProjectSetting<T> : ISetting
        {
            protected ProjectSetting(string name, T defaultValue, ProjectSettingsState projectSettingsState)
            {
                this.name = name;
                this.defaultValue = defaultValue;

                stage = SettingStage.Project;

                _projectSettingsState = projectSettingsState;
            }

            protected ProjectSettingsState _projectSettingsState;

            public string name { get; }
            public string valueString => Get().ToString();
            public T defaultValue { get; }
            public SettingType type { get; protected set; }
            public SettingStage stage { get; }

            public T Get()
            {
                var settingValue = _projectSettingsState.GetSetting<T>(name);

                return settingValue.TryGetValue(out var value) ?
                    value :
                    defaultValue;
            }

            public void Set(T value)
            {
                _projectSettingsState.SetSetting(name, value);
            }
        }

        public class Vec3ProjectSetting : ProjectSetting<Vector3>
        {
            public Vec3ProjectSetting(string name, Vector3 defaultValue, ProjectSettingsState projectSettingsState) : base(name, defaultValue, projectSettingsState)
            {
                type = SettingType.Vector3;
            }
        }

        public class StringProjectSetting : ProjectSetting<string>
        {
            public StringProjectSetting(string name, string defaultValue, ProjectSettingsState projectSettingsState) : base(name, defaultValue, projectSettingsState)
            {
                type = SettingType.Text;
            }
        }

        [Inject]
        public SettingsSystem(ProjectSettingsState projectSettingsState)
        {
            TestNumber = new FloatUserSetting("Test number", 12.34f);
            AllSettings.Add(TestNumber);

            TestInteger = new IntUserSetting("Test integer", 123);
            AllSettings.Add(TestInteger);

            TestString = new StringUserSetting("Test text", "Hello world!");
            AllSettings.Add(TestString);

            TestProjVec3 = new Vec3ProjectSetting("Test Vec3 Project", Vector3.one, projectSettingsState);
            AllSettings.Add(TestProjVec3);

            TestProjString = new StringProjectSetting("Test String Project", "some text", projectSettingsState);
            AllSettings.Add(TestProjString);
        }

        public List<ISetting> AllSettings = new List<ISetting>();

        public FloatUserSetting TestNumber;
        public IntUserSetting TestInteger;
        public StringUserSetting TestString;

        public Vec3ProjectSetting TestProjVec3;
        public StringProjectSetting TestProjString;
    }
}
