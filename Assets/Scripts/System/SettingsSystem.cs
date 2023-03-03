using System;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class SettingsSystem
    {
        public class Setting<T>
        {
            internal Setting(EditorEvents editorEvents, string key, SettingSaver saver, params SettingFilter<T>[] filters)
            {
                this.key = key;
                this.saver = saver;
                this.editorEvents = editorEvents;
                this.filters = filters;
            }

            public string key;

            private SettingSaver saver;

            private EditorEvents editorEvents;

            private SettingFilter<T>[] filters;

            public T Get()
            {
                var value = saver.ReadValue<T>(key);

                foreach (var filter in filters)
                {
                    value = filter.FilterRead(value);
                }

                return value;
            }

            public void Set(T value)
            {
                foreach (var filter in filters)
                {
                    value = filter.FilterWrite(value);
                }

                if (value == null)
                {
                    // Rejecting save
                    return;
                }

                saver.WriteValue<T>(key, value);
                editorEvents.InvokeSettingsChangedEvent();
            }
        }

        public abstract class SettingSaver
        {
            public abstract dynamic ReadValue<T>(string key);

            public abstract void WriteValue<T>(string key, dynamic value);
        }

        public class UserSettingsSaver : SettingSaver
        {
            public override dynamic ReadValue<T>(string key)
            {
                if (!PlayerPrefs.HasKey(key))
                {
                    return null;
                }

                if (typeof(T) == typeof(string))
                {
                    return PlayerPrefs.GetString(key);
                }

                if (typeof(T) == typeof(float))
                {
                    return PlayerPrefs.GetFloat(key);
                }

                if (typeof(T) == typeof(int))
                {
                    return PlayerPrefs.GetInt(key);
                }

                // throw
                throw new Exception($"The type {typeof(T)} can not be handled by the UserSettingsSaver");
            }

            public override void WriteValue<T>(string key, dynamic value)
            {
                if (typeof(T) == typeof(string))
                {
                    PlayerPrefs.SetString(key, value);
                    return;
                }

                if (typeof(T) == typeof(float))
                {
                    PlayerPrefs.SetFloat(key, value);
                    return;
                }

                if (typeof(T) == typeof(int))
                {
                    PlayerPrefs.SetInt(key, value);
                    return;
                }

                // throw
                throw new Exception($"The type {typeof(T)} can not be handled by the UserSettingsSaver");
            }
        }

        public abstract class SettingFilter<T>
        {
            public virtual dynamic FilterRead(dynamic value)
            {
                return value;
            }

            public virtual dynamic FilterWrite(dynamic value)
            {
                return value;
            }
        }

        public class DefaultFilter<T> : SettingFilter<T>
        {
            private readonly T defaultValue;

            public DefaultFilter(T defaultValue)
            {
                this.defaultValue = defaultValue;
            }

            public override dynamic FilterRead(dynamic value)
            {
                return value ?? defaultValue;
            }
        }

        public class ClampMaxFilter<T> : SettingFilter<T>
        {
            private readonly T maxValue;

            public ClampMaxFilter(T maxValue)
            {
                this.maxValue = maxValue;
            }

            public override dynamic FilterWrite(dynamic value)
            {
                if (value == null)
                    return null;

                return value > maxValue ?
                    maxValue :
                    value;
            }
        }

        public class ClampMinFilter<T> : SettingFilter<T>
        {
            private readonly T minValue;

            public ClampMinFilter(T minValue)
            {
                this.minValue = minValue;
            }

            public override dynamic FilterWrite(dynamic value)
            {
                if (value == null)
                    return null;

                return value < minValue ?
                    minValue :
                    value;
            }
        }

        public class OptionsFilter<T> : SettingFilter<T>
        {
            private readonly T[] availableOptions;

            public OptionsFilter(params T[] availableOptions)
            {
                this.availableOptions = availableOptions;
            }

            public override dynamic FilterWrite(dynamic value)
            {
                if (value == null)
                    return null;

                foreach (var option in availableOptions)
                {
                    if (value == option)
                    {
                        // return the value when it is one of the options
                        return value;
                    }
                }

                // return null (fail the write) when value is *not* one of the options
                return null;
            }
        }

        [Inject]
        public SettingsSystem(EditorEvents editorEvents)
        {
            // setup saver
            var userSettingSaverInstance = new UserSettingsSaver();

            uiScalingFactor = new Setting<float>(
                editorEvents,
                "UI Scaling",
                userSettingSaverInstance,
                new DefaultFilter<float>(1.0f),
                new ClampMinFilter<float>(0.5f),
                new ClampMaxFilter<float>(3.0f));

            mouseSensitivity = new Setting<float>(
                editorEvents,
                "Mouse Sensitivity",
                userSettingSaverInstance,
                new DefaultFilter<float>(1.0f),
                new ClampMinFilter<float>(0.1f),
                new ClampMaxFilter<float>(10.0f));

            gizmoSize = new Setting<float>(
                editorEvents,
                "Gizmo Size",
                userSettingSaverInstance,
                new DefaultFilter<float>(1.0f),
                new ClampMinFilter<float>(0.1f),
                new ClampMaxFilter<float>(10.0f));

            applicationTargetFramerate = new Setting<int>(
                editorEvents,
                "Maximum frame rate",
                userSettingSaverInstance,
                new DefaultFilter<int>(120),
                new ClampMinFilter<int>(5),
                new ClampMaxFilter<int>(1000));


            // Saves Panel Size
            panelSize = new Setting<string>(
                editorEvents,
                "Panel Size",
                userSettingSaverInstance);


            // last opened scene
            openLastOpenedScene = new Setting<string>(
                editorEvents,
                "Open last opened scene on start up",
                userSettingSaverInstance);


            // Gizmo Settings
            selectedGizmoTool = new Setting<int>(
                editorEvents,
                "Selected Gizmo Tool",
                userSettingSaverInstance,
                new DefaultFilter<int>(0),
                new OptionsFilter<int>(0, 1, 2));

            gizmoLocalGlobalContext = new Setting<int>(
                editorEvents,
                "Gizmo Local Global Context",
                userSettingSaverInstance,
                new DefaultFilter<int>(0),
                new OptionsFilter<int>(0, 1));

            gizmoToolDoesSnap = new Setting<int>(
                editorEvents,
                "Gizmo Tool Does Snap",
                userSettingSaverInstance,
                new DefaultFilter<int>(0),
                new OptionsFilter<int>(0, 1));


            // snapping settings
            gizmoToolTranslateSnapping = new Setting<float>(
                editorEvents,
                "Gizmo Tool Translate Snapping",
                userSettingSaverInstance,
                new DefaultFilter<float>(0.25f),
                new ClampMinFilter<float>(0f));

            gizmoToolRotateSnapping = new Setting<float>(
                editorEvents,
                "Gizmo Tool Rotate Snapping",
                userSettingSaverInstance,
                new DefaultFilter<float>(15f), // degrees
                new ClampMinFilter<float>(0f));

            gizmoToolScaleSnapping = new Setting<float>(
                editorEvents,
                "Gizmo Tool Scale Snapping",
                userSettingSaverInstance,
                new DefaultFilter<float>(0.25f),
                new ClampMinFilter<float>(0f));
        }


        public Setting<float> uiScalingFactor;
        public Setting<float> mouseSensitivity;
        public Setting<float> gizmoSize;
        public Setting<int> applicationTargetFramerate;
        public Setting<string> openLastOpenedScene;


        /// <summary>
        /// 0 = Translate
        /// 1 = Rotate
        /// 2 = Scale
        /// </summary>
        public Setting<int> selectedGizmoTool;


        /// <summary>
        /// 0 = Local Context
        /// 1 = Global Context
        /// </summary>
        public Setting<int> gizmoLocalGlobalContext;

        /// <summary>
        /// 0 = Does not snap
        /// 1 = Does snap
        /// </summary>
        public Setting<int> gizmoToolDoesSnap;


        public Setting<float> gizmoToolTranslateSnapping;
        public Setting<float> gizmoToolRotateSnapping;
        public Setting<float> gizmoToolScaleSnapping;

        public Setting<string> panelSize;
    }
}
