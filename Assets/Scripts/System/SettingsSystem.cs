using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class SettingsSystem
    {
        public enum SettingType
        {
            Text,
            Number,
            Integer
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

        public SettingsSystem()
        {
            AllSettings.Add(TestNumber);
            AllSettings.Add(TestInteger);
            AllSettings.Add(TestString);
        }

        public List<ISetting> AllSettings = new List<ISetting>();

        public FloatUserSetting TestNumber = new FloatUserSetting("Test number", 12.34f);
        public IntUserSetting TestInteger = new IntUserSetting("Test integer", 123);
        public StringUserSetting TestString = new StringUserSetting("Test text", "Hello world!");
    }
}
