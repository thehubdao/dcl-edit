using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Zenject;
using StreamReader = System.IO.StreamReader;
using System.IO.Pipes;
using Newtonsoft.Json.UnityConverters.Math;

namespace Assets.Scripts.System
{
    public class SettingsSystem
    {
        public abstract class SettingOption<T>
        {
        }

        public interface IReadFilter
        {
            public dynamic FilterRead(dynamic value);
        }

        public interface IWriteFilter
        {
            public dynamic FilterWrite(dynamic value);
        }

        public interface IValidator
        {
            public bool Validate(dynamic value);
        }

        public class Setting<T>
        {
            public Setting(EditorEvents editorEvents, string key, SettingSaver saver, params SettingOption<T>[] options)
            {
                this.key = key;
                this.saver = saver;
                this.editorEvents = editorEvents;

                foreach (var option in options)
                {
                    if (option is IReadFilter readFilterOption)
                    {
                        readFilters.Add(readFilterOption);
                    }

                    if (option is IWriteFilter writeFilterOption)
                    {
                        writeFilters.Add(writeFilterOption);
                    }

                    if (option is IValidator validatorOption)
                    {
                        validators.Add(validatorOption);
                    }
                }
            }

            public string key;

            private SettingSaver saver;

            private EditorEvents editorEvents;

            private List<IReadFilter> readFilters = new List<IReadFilter>();
            private List<IWriteFilter> writeFilters = new List<IWriteFilter>();
            private List<IValidator> validators = new List<IValidator>();


            public T Get()
            {
                var value = saver.ReadValue<T>(key);

                foreach (var filter in readFilters)
                {
                    value = filter.FilterRead(value);
                }

                value ??= default(T);

                return value;
            }

            public void Set(T value)
            {
                value = writeFilters.Aggregate(value, (current, filter) => filter.FilterWrite(current));

                if (!Validate(value))
                {
                    throw new Exception("Setting was not saved, because a validator failed.");
                }

                saver.WriteValue<T>(key, value);
                editorEvents.InvokeSettingsChangedEvent();
            }

            public bool Validate(T value)
            {
                return validators.All(v => v.Validate(value));
            }
        }

        public abstract class SettingSaver
        {
            public abstract dynamic ReadValue<T>(string key);

            public abstract void WriteValue<T>(string key, dynamic value);
        }

        public static class SettingSavers
        {
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

            public class JsonSettingsSaver : SettingSaver
            {
                private readonly string settingsPath;

                private static JsonConverter[] converters = new[] {new Vector3Converter()};

                public JsonSettingsSaver(string path)
                {
                    settingsPath = path;
                }

                public override dynamic ReadValue<T>(string key)
                {
                    try
                    {
                        var stream = File.Open(settingsPath, FileMode.OpenOrCreate, FileAccess.Read);

                        var jObject = JObjectFromStreamOrNull(stream) ?? new JObject();

                        stream.Close();

                        if (!jObject.ContainsKey(key))
                        {
                            return null;
                        }

                        var valueObject = jObject[key];

                        return valueObject!.ToObject<T>();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        return null;
                    }
                }

                public override void WriteValue<T>(string key, dynamic value)
                {
                    try
                    {
                        var fileStream = File.Open(settingsPath, FileMode.OpenOrCreate, FileAccess.Read);

                        var originalObject = JObjectFromStreamOrNull(fileStream) ?? new JObject();

                        fileStream.Close();

                        originalObject[key] = JToken.FromObject(value, JsonSerializer.Create(new JsonSerializerSettings() {Converters = converters}));

                        var newJsonString = JsonConvert.SerializeObject(originalObject, Formatting.Indented, converters);

                        File.WriteAllText(settingsPath, newJsonString);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }

                [CanBeNull]
                private JObject JObjectFromStreamOrNull(Stream stream)
                {
                    try
                    {
                        return JObject.Load(new JsonTextReader(new StreamReader(stream)));
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
        }

        public static class SettingOptions
        {
            public class Default<T> : SettingOption<T>, IReadFilter
            {
                private readonly T defaultValue;

                public Default(T defaultValue)
                {
                    this.defaultValue = defaultValue;
                }

                public dynamic FilterRead(dynamic value)
                {
                    return value ?? defaultValue;
                }
            }

            public class ClampMax<T> : SettingOption<T>, IWriteFilter, IValidator
            {
                private readonly T maxValue;

                public ClampMax(T maxValue)
                {
                    this.maxValue = maxValue;
                }

                public dynamic FilterWrite(dynamic value)
                {
                    if (value == null)
                        return null;

                    return value > maxValue ?
                        maxValue :
                        value;
                }

                public bool Validate(dynamic value)
                {
                    return value <= maxValue;
                }
            }

            public class ClampMin<T> : SettingOption<T>, IWriteFilter, IValidator
            {
                private readonly T minValue;

                public ClampMin(T minValue)
                {
                    this.minValue = minValue;
                }

                public dynamic FilterWrite(dynamic value)
                {
                    if (value == null)
                        return null;

                    return value < minValue ?
                        minValue :
                        value;
                }

                public bool Validate(dynamic value)
                {
                    return value >= minValue;
                }
            }

            public class Options<T> : SettingOption<T>, IValidator
            {
                private readonly T[] availableOptions;

                public Options(params T[] availableOptions)
                {
                    this.availableOptions = availableOptions;
                }

                public bool Validate(dynamic value)
                {
                    return availableOptions.Any(option => value == option);
                }
            }

            public class NotNull<T> : SettingOption<T>, IValidator
            {
                public bool Validate(dynamic value)
                {
                    return value != null;
                }
            }
        }


        [Inject]
        public void Construct(EditorEvents editorEvents, FileManagerSystem fileManagerSystem)
        {
            // setup saver
            var userSettingSaverInstance = new SettingSavers.UserSettingsSaver();
            var projectSettingsSaverInstance = new SettingSavers.JsonSettingsSaver(fileManagerSystem.GetFilePath("dcl-edit/settings.json"));

            uiScalingFactor = new Setting<float>(
                editorEvents,
                "UI Scaling",
                userSettingSaverInstance,
                new SettingOptions.Default<float>(1.0f),
                new SettingOptions.ClampMin<float>(0.5f),
                new SettingOptions.ClampMax<float>(3.0f));

            mouseSensitivity = new Setting<float>(
                editorEvents,
                "Mouse Sensitivity",
                userSettingSaverInstance,
                new SettingOptions.Default<float>(1.0f),
                new SettingOptions.ClampMin<float>(0.1f),
                new SettingOptions.ClampMax<float>(10.0f));

            gizmoSize = new Setting<float>(
                editorEvents,
                "Gizmo Size",
                userSettingSaverInstance,
                new SettingOptions.Default<float>(1.0f),
                new SettingOptions.ClampMin<float>(0.1f),
                new SettingOptions.ClampMax<float>(10.0f));

            applicationTargetFramerate = new Setting<int>(
                editorEvents,
                "Maximum frame rate",
                userSettingSaverInstance,
                new SettingOptions.Default<int>(120),
                new SettingOptions.ClampMin<int>(5),
                new SettingOptions.ClampMax<int>(1000));


            // Saves Panel Size
            panelSize = new Setting<string>(
                editorEvents,
                "Panel Size",
                userSettingSaverInstance,
                new SettingOptions.NotNull<string>());


            // last opened scene
            openLastOpenedScene = new Setting<string>(
                editorEvents,
                "Open last opened scene on start up",
                projectSettingsSaverInstance);


            // Gizmo Settings
            selectedGizmoTool = new Setting<int>(
                editorEvents,
                "Selected Gizmo Tool",
                userSettingSaverInstance,
                new SettingOptions.Default<int>(0),
                new SettingOptions.Options<int>(0, 1, 2));

            gizmoLocalGlobalContext = new Setting<int>(
                editorEvents,
                "Gizmo Local Global Context",
                userSettingSaverInstance,
                new SettingOptions.Default<int>(0),
                new SettingOptions.Options<int>(0, 1));

            gizmoToolDoesSnap = new Setting<int>(
                editorEvents,
                "Gizmo Tool Does Snap",
                userSettingSaverInstance,
                new SettingOptions.Default<int>(0),
                new SettingOptions.Options<int>(0, 1));


            // snapping settings
            gizmoToolTranslateSnapping = new Setting<float>(
                editorEvents,
                "Gizmo Tool Translate Snapping",
                userSettingSaverInstance,
                new SettingOptions.Default<float>(0.25f),
                new SettingOptions.ClampMin<float>(0f));

            gizmoToolRotateSnapping = new Setting<float>(
                editorEvents,
                "Gizmo Tool Rotate Snapping",
                userSettingSaverInstance,
                new SettingOptions.Default<float>(15f), // degrees
                new SettingOptions.ClampMin<float>(0f));

            gizmoToolScaleSnapping = new Setting<float>(
                editorEvents,
                "Gizmo Tool Scale Snapping",
                userSettingSaverInstance,
                new SettingOptions.Default<float>(0.25f),
                new SettingOptions.ClampMin<float>(0f));

            skyboxTime = new Setting<float>(
                editorEvents,
                "Skybox Time",
                projectSettingsSaverInstance,
                new SettingOptions.Default<float>(12f),
                new SettingOptions.ClampMin<float>(0f),
                new SettingOptions.ClampMax<float>(24f)
                );

            groundGridSetting = new Setting<int>(
                editorEvents,
                "Change Grid",
                userSettingSaverInstance,
                new SettingOptions.Default<int>(1),
                new SettingOptions.ClampMin<int>(0),
                new SettingOptions.ClampMax<int>(2));

            groundGridSizeSetting = new Setting<int>(
                editorEvents,
                "Change GridSize",
                userSettingSaverInstance,
                new SettingOptions.Default<int>(16),
                new SettingOptions.ClampMin<int>(2),
                new SettingOptions.ClampMax<int>(100));
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


        /// <summary>
        /// 0 = No grid
        /// 1 = Old grid
        /// 2 = New grid
        /// </summary>
        public Setting<int> groundGridSetting;
        public Setting<int> groundGridSizeSetting;

        public Setting<float> gizmoToolTranslateSnapping;
        public Setting<float> gizmoToolRotateSnapping;
        public Setting<float> gizmoToolScaleSnapping;

        public Setting<string> panelSize;

        /// <summary>
        /// Time of day for the skybox in 24h format
        /// </summary>
        public Setting<float> skyboxTime;
    }
}
