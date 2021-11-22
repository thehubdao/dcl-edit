using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class GizmoRelationManager : Manager
{
    public enum RelationSettingEnum
    {
        Local,
        Global
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
    public static void SwitchToNextRelationSetting()
    {
        RelationSetting = RelationSetting.Next();
        onUpdate.Invoke();
    }
    
}

public static class GizmoManagerHelper
{
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
