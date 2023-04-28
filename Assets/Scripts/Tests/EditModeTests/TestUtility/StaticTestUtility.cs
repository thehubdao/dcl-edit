using Assets.Scripts.System;
using Newtonsoft.Json.Linq;

public static class StaticTestUtility
{
    public static JObject WithImportFile(this JObject jObject, string importFile)
    {
        jObject[CustomComponentMarkupSystem.importFileKey] = importFile;
        return jObject;
    }
}
