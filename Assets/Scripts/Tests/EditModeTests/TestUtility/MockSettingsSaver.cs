using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Scripts.System;

namespace Assets.Scripts.Tests.EditModeTests.TestUtility
{
    public class MockSettingsSaver : SettingsSystem.SettingSaver
    {
        private readonly Dictionary<string, dynamic> settings = new Dictionary<string, dynamic>();

        public override dynamic ReadValue<T>(string key)
        {
            return settings.TryGetValue(key, out var value) ? value : null;
        }

        public override void WriteValue<T>(string key, dynamic value)
        {
            settings[key] = value;
        }
    }
}
