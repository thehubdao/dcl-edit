using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;

public class GizmoRelationManager : Manager
{
    public enum RelationSettingEnum
    {
        Local,
        Global
    }

    public static RelationSettingEnum RelationSetting
    {
        get => PersistentData.GizmoRelation switch
        {
            0 => RelationSettingEnum.Local,
            1 => RelationSettingEnum.Global,
            _ => throw new IndexOutOfRangeException()
        };
        set
        {
            PersistentData.GizmoRelation = value switch
            {
                RelationSettingEnum.Local => 0,
                RelationSettingEnum.Global => 1,
                _ => throw new IndexOutOfRangeException()
            };
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
