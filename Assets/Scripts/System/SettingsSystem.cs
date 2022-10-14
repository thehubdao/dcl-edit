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

        public abstract class JsonSetting<T, TSettingState> : ISetting where TSettingState : JsonSettingState
        {
            protected JsonSetting(string name, T defaultValue, TSettingState tSettingState)
            {
                this.name = name;
                this.defaultValue = defaultValue;

                if (typeof(TSettingState) == typeof(ProjectSettingState))
                {
                    stage = SettingStage.Project;
                }

                if (typeof(TSettingState) == typeof(SceneSettingState))
                {
                    stage = SettingStage.Scene;
                }

                SettingState = tSettingState;
            }

            protected TSettingState SettingState;

            public string name { get; }
            public string valueString => Get().ToString();
            public T defaultValue { get; }
            public SettingType type { get; protected set; }
            public SettingStage stage { get; }

            public T Get()
            {
                var settingValue = SettingState.GetSetting<T>(name);

                return settingValue.TryGetValue(out var value) ?
                    value :
                    defaultValue;
            }

            public void Set(T value)
            {
                SettingState.SetSetting(name, value);
            }
        }

        public class Vec3ProjectSetting : JsonSetting<Vector3, ProjectSettingState>
        {
            public Vec3ProjectSetting(string name, Vector3 defaultValue, ProjectSettingState projectSettingsState) : base(name, defaultValue, projectSettingsState)
            {
                type = SettingType.Vector3;
            }
        }


        public class StringProjectSetting : JsonSetting<string, ProjectSettingState>
        {
            public StringProjectSetting(string name, string defaultValue, ProjectSettingState projectSettingsState) : base(name, defaultValue, projectSettingsState)
            {
                type = SettingType.Text;
            }
        }

        public class Vec3SceneSetting : JsonSetting<Vector3, SceneSettingState>
        {
            public Vec3SceneSetting(string name, Vector3 defaultValue, SceneSettingState projectSettingsState) : base(name, defaultValue, projectSettingsState)
            {
                type = SettingType.Vector3;
            }
        }


        [Inject]
        public SettingsSystem(ProjectSettingState projectSettingsState, SceneSettingState sceneSettingState)
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

            TestSceneVec3 = new Vec3SceneSetting("Test Vec3 Scene", Vector3.one, sceneSettingState);
            AllSettings.Add(TestSceneVec3);
        }

        public List<ISetting> AllSettings = new List<ISetting>();

        public FloatUserSetting TestNumber;
        public IntUserSetting TestInteger;
        public StringUserSetting TestString;

        public Vec3ProjectSetting TestProjVec3;
        public StringProjectSetting TestProjString;

        public Vec3SceneSetting TestSceneVec3;
    }
}
