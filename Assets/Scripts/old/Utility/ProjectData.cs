using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectData
{

    [Serializable]
    public class JSON
    {
        public float translateSnapStep;
        public float rotateSnapStep;
        public float scaleSnapStep;

        public string generateScriptLocation;

        public JSON()
        {
            translateSnapStep = ProjectData.translateSnapStep;
            rotateSnapStep = ProjectData.rotateSnapStep;
            scaleSnapStep = ProjectData.scaleSnapStep;

            generateScriptLocation = ProjectData.generateScriptLocation;
        }
    }

    public static void ApplyJsonString(string jsonString)
    {
        ApplyJson(JsonUtility.FromJson<JSON>(jsonString));
    }

    public static void ApplyJson(JSON json)
    {

        translateSnapStep = json.translateSnapStep;
        rotateSnapStep = json.rotateSnapStep;
        scaleSnapStep = json.scaleSnapStep;

        generateScriptLocation = json.generateScriptLocation;
    }

    public static string ToJson()
    {
        return JsonUtility.ToJson(new JSON(),true);
    }
    
    public static float translateSnapStep = 0.25f;
    public static float rotateSnapStep = 15f;
    public static float scaleSnapStep = 0.25f;

    public static string generateScriptLocation = "src/scene.ts";

}
