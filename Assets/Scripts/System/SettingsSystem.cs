using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class SettingsSystem
    {
        //Dependencies
        EditorEvents _editorEvents;

        public enum SettingType
        {
            String,
            Float,
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
            // Dependencies
            private SettingsSystem _settingsSystem;

            protected UserSetting(SettingsSystem settingsSystem, string name, T defaultValue)
            {
                _settingsSystem = settingsSystem;

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
            public virtual void Set(T value)
            {
                _settingsSystem._editorEvents.InvokeSettingsChangedEvent();
            }
        }

        public class StringUserSetting : UserSetting<string>
        {
            public StringUserSetting(SettingsSystem settingsSystem, string name, string defaultValue) : base(settingsSystem, name, defaultValue)
            {
                type = SettingType.String;
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
                base.Set(value);
            }
        }

        public class IntUserSetting : UserSetting<int>
        {
            public IntUserSetting(SettingsSystem settingsSystem, string name, int defaultValue) : base(settingsSystem, name, defaultValue)
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
                base.Set(value);
            }
        }

        public class IntClampedUserSetting : IntUserSetting
        {
            private int minValue;
            private int maxValue;

            public IntClampedUserSetting(SettingsSystem settingsSystem, string name, int defaultValue, int minValue, int maxValue) : base(settingsSystem, name, defaultValue)
            {
                this.minValue = minValue;
                this.maxValue = maxValue;
            }

            public override void Set(int value)
            {
                base.Set(Mathf.Clamp(value, minValue, maxValue));
            }
        }

        public class FloatUserSetting : UserSetting<float>
        {
            public FloatUserSetting(SettingsSystem settingsSystem, string name, float defaultValue) : base(settingsSystem, name, defaultValue)
            {
                type = SettingType.Float;
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
                base.Set(value);
            }
        }

        public class FloatClampedUserSetting : FloatUserSetting
        {
            private float minValue;
            private float maxValue;

            public FloatClampedUserSetting(SettingsSystem settingsSystem, string name, float defaultValue, float minValue, float maxValue) : base(settingsSystem, name, defaultValue)
            {
                this.minValue = minValue;
                this.maxValue = maxValue;
            }

            public override void Set(float value)
            {
                base.Set(Mathf.Clamp(value, minValue, maxValue));
            }
        }

        public abstract class JsonSetting<T, TSettingState> : ISetting where TSettingState : JsonSettingState
        {
            // Dependencies
            private SettingsSystem _settingsSystem;

            protected JsonSetting(SettingsSystem settingsSystem, string name, T defaultValue, TSettingState tSettingState)
            {
                _settingsSystem = settingsSystem;

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
                _settingsSystem._editorEvents.InvokeSettingsChangedEvent();
            }
        }

        public class Vec3ProjectSetting : JsonSetting<Vector3, ProjectSettingState>
        {
            public Vec3ProjectSetting(SettingsSystem settingsSystem, string name, Vector3 defaultValue, ProjectSettingState projectSettingsState) : base(settingsSystem, name, defaultValue, projectSettingsState)
            {
                type = SettingType.Vector3;
            }
        }


        public class StringProjectSetting : JsonSetting<string, ProjectSettingState>
        {
            public StringProjectSetting(SettingsSystem settingsSystem, string name, string defaultValue, ProjectSettingState projectSettingsState) : base(settingsSystem, name, defaultValue, projectSettingsState)
            {
                type = SettingType.String;
            }
        }

        public class Vec3SceneSetting : JsonSetting<Vector3, SceneSettingState>
        {
            public Vec3SceneSetting(SettingsSystem settingsSystem, string name, Vector3 defaultValue, SceneSettingState projectSettingsState) : base(settingsSystem, name, defaultValue, projectSettingsState)
            {
                type = SettingType.Vector3;
            }
        }


        [Inject]
        public SettingsSystem(ProjectSettingState projectSettingsState, SceneSettingState sceneSettingState, EditorEvents editorEvents)
        {
            _editorEvents = editorEvents;

            var userSettings = new List<ISetting>();

            uiScalingFactor = new FloatClampedUserSetting(this, "UI Scaling", 1.0f, 0.5f, 3.0f);
            userSettings.Add(uiScalingFactor);

            mouseSensitivity = new FloatClampedUserSetting(this, "Mouse Sensitivity", 1.0f, 0.1f, 10.0f);
            userSettings.Add(mouseSensitivity);

            gizmoSize = new FloatClampedUserSetting(this, "Gizmo Size", 1.0f, 0.1f, 10.0f);
            userSettings.Add(gizmoSize);

            applicationTargetFramerate = new IntClampedUserSetting(this, "Maximum frame rate", 120, 5, 1000);
            userSettings.Add(applicationTargetFramerate);

            openLastOpenedScene = new StringUserSetting(this, "Open last opened scene on start up", "");
            //userSettings.Add(openLastOpenedScene);

            ShownSettings.Add("User Settings", userSettings);

            //Hidden Settings
            //Saves Panel Size
            panelSize = new StringUserSetting(this, "Panel Size","");
        }

        public Dictionary<string, List<ISetting>> ShownSettings = new Dictionary<string, List<ISetting>>();

        public FloatClampedUserSetting uiScalingFactor;
        public FloatClampedUserSetting mouseSensitivity;
        public FloatClampedUserSetting gizmoSize;
        public IntUserSetting TestInteger;
        public StringUserSetting TestString;
        public IntClampedUserSetting applicationTargetFramerate;
        public StringUserSetting openLastOpenedScene;

        public Vec3ProjectSetting TestProjVec3;
        public StringProjectSetting TestProjString;

        public Vec3SceneSetting TestSceneVec3;

        public StringUserSetting panelSize;
    }
}
