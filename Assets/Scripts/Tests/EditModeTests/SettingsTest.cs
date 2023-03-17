using Assets.Scripts.Events;
using Assets.Scripts.Tests.EditModeTests.TestUtility;
using NUnit.Framework;
using System;
using UnityEngine;
using static Assets.Scripts.System.SettingsSystem;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class SettingsTest
    {
        [Test]
        public void BasicSetup()
        {
            var editorEvents = new EditorEvents();

            var mockSaver = new MockSettingsSaver();

            var setting = new Setting<int>(
                editorEvents,
                "test int user setting",
                mockSaver);

            Assert.AreEqual(0, setting.Get());

            setting.Set(26);

            Assert.AreEqual(26, setting.Get());

            Assert.IsTrue(setting.Validate(30));
        }

        [Test]
        public void DifferentTypes()
        {
            var editorEvents = new EditorEvents();

            var mockSaver = new MockSettingsSaver();

            var intSetting = new Setting<int>(
                editorEvents,
                "test int user setting",
                mockSaver);

            var floatSetting = new Setting<float>(
                editorEvents,
                "test float user setting",
                mockSaver);

            var stringSetting = new Setting<string>(
                editorEvents,
                "test string user setting",
                mockSaver);

            var vector3Setting = new Setting<Vector3>(
                editorEvents,
                "test vector3 user setting",
                mockSaver);

            Assert.AreEqual(0, intSetting.Get());
            Assert.AreEqual(0f, floatSetting.Get());
            Assert.AreEqual(null, stringSetting.Get());
            Assert.AreEqual(Vector3.zero, vector3Setting.Get());

            intSetting.Set(26);
            floatSetting.Set(26.5f);
            stringSetting.Set("26");
            vector3Setting.Set(new Vector3(26, 26, 26));

            Assert.AreEqual(26, intSetting.Get());
            Assert.AreEqual(26.5f, floatSetting.Get());
            Assert.AreEqual("26", stringSetting.Get());
            Assert.AreEqual(new Vector3(26, 26, 26), vector3Setting.Get());

            Assert.IsTrue(intSetting.Validate(30));
            Assert.IsTrue(floatSetting.Validate(30.5f));
            Assert.IsTrue(stringSetting.Validate("30"));
            Assert.IsTrue(vector3Setting.Validate(new Vector3(30, 30, 30)));
        }

        [Test]
        public void DefaultValue()
        {
            var editorEvents = new EditorEvents();

            var mockSaver = new MockSettingsSaver();

            var intSetting = new Setting<int>(
                editorEvents,
                "test int user setting",
                mockSaver,
                new SettingOptions.Default<int>(20));

            Assert.AreEqual(20, intSetting.Get());

            intSetting.Set(26);

            Assert.AreEqual(26, intSetting.Get());

            var vector3Setting = new Setting<Vector3>(
                editorEvents,
                "test vector3 user setting",
                mockSaver,
                new SettingOptions.Default<Vector3>(new Vector3(20, 20, 20)));

            Assert.AreEqual(new Vector3(20, 20, 20), vector3Setting.Get());

            vector3Setting.Set(new Vector3(26, 26, 26));

            Assert.AreEqual(new Vector3(26, 26, 26), vector3Setting.Get());
        }

        [Test]
        public void ClampMin()
        {
            var editorEvents = new EditorEvents();

            var mockSaver = new MockSettingsSaver();

            var intSetting = new Setting<int>(
                editorEvents,
                "test int user setting",
                mockSaver,
                new SettingOptions.ClampMin<int>(-40));
            var floatSetting = new Setting<float>(
                editorEvents,
                "test float user setting",
                mockSaver,
                new SettingOptions.ClampMin<float>(-40f));

            Assert.AreEqual(0, intSetting.Get());
            Assert.AreEqual(0f, floatSetting.Get());

            intSetting.Set(26);
            floatSetting.Set(26.5f);

            Assert.AreEqual(26, intSetting.Get());
            Assert.AreEqual(26.5f, floatSetting.Get());

            intSetting.Set(-26);
            floatSetting.Set(-26.5f);

            Assert.AreEqual(-26, intSetting.Get());
            Assert.AreEqual(-26.5f, floatSetting.Get());

            intSetting.Set(-60);
            floatSetting.Set(-60.5f);

            Assert.AreEqual(-40, intSetting.Get());
            Assert.AreEqual(-40f, floatSetting.Get());

            intSetting.Set(60);
            floatSetting.Set(60.5f);

            Assert.AreEqual(60, intSetting.Get());
            Assert.AreEqual(60.5f, floatSetting.Get());

            Assert.IsTrue(intSetting.Validate(30));
            Assert.IsTrue(floatSetting.Validate(30.5f));

            Assert.IsFalse(intSetting.Validate(-50));
            Assert.IsFalse(floatSetting.Validate(-50.5f));

            Assert.IsTrue(intSetting.Validate(50));
            Assert.IsTrue(floatSetting.Validate(50.5f));
        }

        [Test]
        public void ClampMax()
        {
            var editorEvents = new EditorEvents();

            var mockSaver = new MockSettingsSaver();

            var intSetting = new Setting<int>(
                editorEvents,
                "test int user setting",
                mockSaver,
                new SettingOptions.ClampMax<int>(40));
            var floatSetting = new Setting<float>(
                editorEvents,
                "test float user setting",
                mockSaver,
                new SettingOptions.ClampMax<float>(40f));

            Assert.AreEqual(0, intSetting.Get());
            Assert.AreEqual(0f, floatSetting.Get());

            intSetting.Set(26);
            floatSetting.Set(26.5f);

            Assert.AreEqual(26, intSetting.Get());
            Assert.AreEqual(26.5f, floatSetting.Get());

            intSetting.Set(-26);
            floatSetting.Set(-26.5f);

            Assert.AreEqual(-26, intSetting.Get());
            Assert.AreEqual(-26.5f, floatSetting.Get());

            intSetting.Set(60);
            floatSetting.Set(60.5f);

            Assert.AreEqual(40, intSetting.Get());
            Assert.AreEqual(40f, floatSetting.Get());

            intSetting.Set(-60);
            floatSetting.Set(-60.5f);

            Assert.AreEqual(-60, intSetting.Get());
            Assert.AreEqual(-60.5f, floatSetting.Get());

            Assert.IsTrue(intSetting.Validate(30));
            Assert.IsTrue(floatSetting.Validate(30.5f));

            Assert.IsTrue(intSetting.Validate(-50));
            Assert.IsTrue(floatSetting.Validate(-50.5f));

            Assert.IsFalse(intSetting.Validate(50));
            Assert.IsFalse(floatSetting.Validate(50.5f));
        }

        [Test]
        public void Options()
        {
            var editorEvents = new EditorEvents();

            var mockSaver = new MockSettingsSaver();

            var intSetting = new Setting<int>(
                editorEvents,
                "test int user setting",
                mockSaver,
                new SettingOptions.Options<int>(1, 2, 3, 4, 5));
            var stringSetting = new Setting<string>(
                editorEvents,
                "test string user setting",
                mockSaver,
                new SettingOptions.Options<string>("one", "two", "three", "four", "five"));

            Assert.AreEqual(0, intSetting.Get());
            Assert.AreEqual(null, stringSetting.Get());

            intSetting.Set(1);
            stringSetting.Set("one");

            Assert.AreEqual(1, intSetting.Get());
            Assert.AreEqual("one", stringSetting.Get());

            Assert.Throws<Exception>(() => intSetting.Set(6));
            Assert.Throws<Exception>(() => stringSetting.Set("six"));

            Assert.Throws<Exception>(() => intSetting.Set(-30));
            Assert.Throws<Exception>(() => stringSetting.Set("negative thirty"));

            Assert.Throws<Exception>(() => stringSetting.Set(null));

            Assert.IsTrue(intSetting.Validate(1));
            Assert.IsTrue(stringSetting.Validate("one"));

            Assert.IsFalse(intSetting.Validate(6));
            Assert.IsFalse(stringSetting.Validate("six"));

            Assert.IsFalse(intSetting.Validate(-30));
            Assert.IsFalse(stringSetting.Validate("negative thirty"));

            Assert.IsFalse(stringSetting.Validate(null));

            var stringWithNullSetting = new Setting<string>(
                editorEvents,
                "test with null string user setting",
                mockSaver,
                new SettingOptions.Options<string>("one", "two", "three", "four", "five", null));

            Assert.AreEqual(null, stringWithNullSetting.Get());

            stringWithNullSetting.Set("three");

            Assert.AreEqual("three", stringWithNullSetting.Get());

            stringWithNullSetting.Set(null);

            Assert.AreEqual(null, stringWithNullSetting.Get());

            Assert.Throws<Exception>(() => stringSetting.Set("null"));

            Assert.IsTrue(stringWithNullSetting.Validate(null));
            Assert.IsTrue(stringWithNullSetting.Validate("one"));
            Assert.IsFalse(stringWithNullSetting.Validate("null"));
        }
    }
}
