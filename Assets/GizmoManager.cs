using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class GizmoManager : MonoBehaviour
{
    public enum RelationSetting
    {
        Local,
        Global
    }

    public RelationSetting relationSetting;
    public TextMeshProUGUI relationSettingText;
    public void SwitchToNextRelationSetting()
    {
        relationSetting = relationSetting.Next();
        relationSettingText.text = relationSetting.ToString();
        UpdateGizmoOrientation();
    }


    public void UpdateGizmoOrientation()
    {
        if(SceneManager.SelectedEntity!=null)
        {
            switch (relationSetting)
            {
                case RelationSetting.Local:
                    SceneManager.SelectedEntity.gizmos.transform.localRotation = Quaternion.identity;
                    break;
                case RelationSetting.Global:
                    SceneManager.SelectedEntity.gizmos.transform.rotation = Quaternion.identity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

public static class GizmoManagerHelper
{
    public static T Next<T>(this T src) where T : Enum
    {
        if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

        T[] arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(arr, src) + 1;
        return (arr.Length==j) ? arr[0] : arr[j];            
    }

    public static Space ToSpace(this GizmoManager.RelationSetting setting)
    {
        return setting switch
        {
            GizmoManager.RelationSetting.Local => Space.Self,
            GizmoManager.RelationSetting.Global => Space.World,
            _ => throw new ArgumentOutOfRangeException(nameof(setting), setting, null)
        };
    }
}
