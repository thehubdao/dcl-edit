using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersistentData
{
    /*
    public static float SOMENAME
    {
        set => PlayerPrefs.SetFloat("SOMENAME",value);
        get => !PlayerPrefs.HasKey("SOMENAME") ? [Default value] : PlayerPrefs.GetFloat("SOMENAME");
    }
    public static int SOMENAME
    {
        set => PlayerPrefs.SetInt("SOMENAME",value);
        get => !PlayerPrefs.HasKey("SOMENAME") ? [Default value] : PlayerPrefs.GetInt("SOMENAME");
    }
    public static string SOMENAME
    {
        set => PlayerPrefs.SetString("SOMENAME",value);
        get => !PlayerPrefs.HasKey("SOMENAME") ? [Default value] : PlayerPrefs.GetString("SOMENAME");
    }
    */

    // Template: peda

    private static Data<float> _cameraSpeedData = MakeNewFloatData("CameraSpeed", 20f);
    public static float CameraSpeed
    {
        set => _cameraSpeedData.Set(value);
        get => _cameraSpeedData.Get();
    }

    private static Data<float> _mouseSensitivityData = MakeNewFloatData("MouseSensitivity", 3f);

    public static float MouseSensitivity
    {
        set => _mouseSensitivityData.Set(value);
        get => _mouseSensitivityData.Get();
    }

    private static Data<float> _gizmoSizeData = MakeNewFloatData("GizmoSize", 0.15f);

    public static float GizmoSize
    {
        set => _gizmoSizeData.Set(value);
        get => _gizmoSizeData.Get();
    }

    private static Data<int> _currentToolData = MakeNewIntData("CurrentTool", 0);

    public static int CurrentTool
    {
        set => _currentToolData.Set(value);
        get => _currentToolData.Get();
    }

    private static Data<int> _gizmoRelationData = MakeNewIntData("GizmoRelation", 1);

    public static int GizmoRelation
    {
        set => _gizmoRelationData.Set(value);
        get => _gizmoRelationData.Get();
    }

    private static Data<int> _isSnappingData = MakeNewIntData("IsSnapping", 1);

    public static int IsSnapping
    {
        set => _isSnappingData.Set(value);
        get => _isSnappingData.Get();
    }


    private static Data<float> _uiScaleData = MakeNewFloatData("UiScale", 1f);

    public static float UiScale
    {
        set => _uiScaleData.Set(value);
        get => _uiScaleData.Get();
    }
    private static Data<int> _numberOfBackups = MakeNewIntData("NumberOfBackups", -1);
    public static int NumberOfBackups
        {
            set=>_numberOfBackups.Set(value);
            get=> _numberOfBackups.Get();
        }


    private static Data<int> MakeNewIntData(string key, int defaultValue)
    {
        return new Data<int>(key, defaultValue, PlayerPrefs.GetInt, PlayerPrefs.SetInt);
    }

    private static Data<string> MakeNewStringData(string key, string defaultValue)
    {
        return new Data<string>(key, defaultValue, PlayerPrefs.GetString, PlayerPrefs.SetString);
    }

    private static Data<float> MakeNewFloatData(string key, float defaultValue)
    {
        return new Data<float>(key, defaultValue, PlayerPrefs.GetFloat, PlayerPrefs.SetFloat);
    }

    private class Data<T>
    {
        private string _key;

        private T _value;

        private Action<string,T> _setFunc;

        public Data(string key, T defaultValue, Func<string,T> getFunc,Action<string,T> setFunc)
        {
            _value = !PlayerPrefs.HasKey(key) ? defaultValue : getFunc(key);

            _setFunc = setFunc;
        }

        public T Get()
        {
            return _value;
        }

        public void Set(T value)
        {
            _value = value;
            _setFunc(_key,value);
        }
    }
}
