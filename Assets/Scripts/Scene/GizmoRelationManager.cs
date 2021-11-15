using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class GizmoRelationManager : MonoBehaviour
{
    public enum RelationSettingEnum
    {
        Local,
        Global
    }

    public static GizmoRelationManager instance;

    void Start()
    {
        instance = this;
    }
    
    [SerializeField]
    private static RelationSettingEnum _relationSetting;

    public static RelationSettingEnum RelationSetting
    {
        get => _relationSetting;
        set
        {
            _relationSetting = value;
            onUpdate.Invoke();
        }
    }
    
    
    //public TextMeshProUGUI relationSettingText;
    public static UnityEvent onUpdate = new UnityEvent();
    public void SwitchToNextRelationSetting()
    {
        RelationSetting = RelationSetting.Next();
        //relationSettingText.text = relationSetting.ToString();
        onUpdate.Invoke();
        //UpdateGizmoOrientation();
    }


    //public void UpdateGizmoOrientation()
    //{
    //    if(SceneManager.SelectedEntity!=null)
    //    {
    //        switch (relationSetting)
    //        {
    //            case RelationSetting.Local:
    //                SceneManager.SelectedEntity.gizmos.transform.localRotation = Quaternion.identity;
    //                break;
    //            case RelationSetting.Global:
    //                SceneManager.SelectedEntity.gizmos.transform.rotation = Quaternion.identity;
    //                break;
    //            default:
    //                throw new ArgumentOutOfRangeException();
    //        }
    //    }
    //}
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

    public static Space ToSpace(this GizmoRelationManager.RelationSettingEnum settingEnum)
    {
        return settingEnum switch 
        {
            GizmoRelationManager.RelationSettingEnum.Local => Space.Self,
            GizmoRelationManager.RelationSettingEnum.Global => Space.World,
            _ => throw new ArgumentOutOfRangeException(nameof(settingEnum), settingEnum, null)
        };
    }
}
