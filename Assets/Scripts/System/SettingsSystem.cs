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


            uiScalingFactor = new FloatClampedUserSetting(this, "UI Scaling", 1.0f, 0.5f, 3.0f);
            mouseSensitivity = new FloatClampedUserSetting(this, "Mouse Sensitivity", 1.0f, 0.1f, 10.0f);
            gizmoSize = new FloatClampedUserSetting(this, "Gizmo Size", 1.0f, 0.1f, 10.0f);
            applicationTargetFramerate = new IntClampedUserSetting(this, "Maximum frame rate", 120, 5, 1000);


            ShownSettings.Add(
                "User Settings",
                new List<ISetting>
                {
                    uiScalingFactor,
                    mouseSensitivity,
                    gizmoSize,
                    applicationTargetFramerate
                });

            //Hidden Settings
            //Saves Panel Size
            panelSize = new StringUserSetting(this, "Panel Size","");


            //Gizmo Settings
            selectedGizmoTool = new IntUserSetting(this, "Selected Gizmo Tool", 0);
            gizmoLocalGlobalContext = new IntUserSetting(this, "Gizmo Local Global Context", 0);
            gizmoToolDoesSnap = new IntUserSetting(this, "Gizmo Tool Does Snap", 0);

            ShownSettings.Add(
                "Tmp Gizmo Settings",
                new List<ISetting>
                {
                    gizmoLocalGlobalContext,
                    gizmoToolDoesSnap
                });

            gizmoToolTranslateSnapping = new FloatUserSetting(this, "Gizmo Tool Translate Snapping", 0.25f);
            gizmoToolRotateSnapping = new FloatUserSetting(this, "Gizmo Tool Rotate Snapping", 15f); // degrees
            gizmoToolScaleSnapping = new FloatUserSetting(this, "Gizmo Tool Scale Snapping", 0.25f);

            ShownSettings.Add(
                "Gizmo Tool Snapping",
                new List<ISetting>
                {
                    gizmoToolTranslateSnapping,
                    gizmoToolRotateSnapping,
                    gizmoToolScaleSnapping
                });
        }

        public Dictionary<string, List<ISetting>> ShownSettings = new Dictionary<string, List<ISetting>>();

        public FloatClampedUserSetting uiScalingFactor;
        public FloatClampedUserSetting mouseSensitivity;
        public FloatClampedUserSetting gizmoSize;
        public IntClampedUserSetting applicationTargetFramerate;


        /// <summary>
        /// 0 = Translate
        /// 1 = Rotate
        /// 2 = Scale
        /// </summary>
        public IntUserSetting selectedGizmoTool;

        /// <summary>
        /// 0 = Local Context
        /// 1 = Global Context
        /// </summary>
        public IntUserSetting gizmoLocalGlobalContext;

        /// <summary>
        /// 0 = Does not snap
        /// 1 = Does snap
        /// </summary>
        public IntUserSetting gizmoToolDoesSnap;


        public FloatUserSetting gizmoToolTranslateSnapping;
        public FloatUserSetting gizmoToolRotateSnapping;
        public FloatUserSetting gizmoToolScaleSnapping;

        public StringUserSetting panelSize;
    }
}
