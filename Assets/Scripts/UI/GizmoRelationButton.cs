using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GizmoRelationButton : MonoBehaviour
{
    [SerializeField]
    private GizmoRelationManager relationManager;

    [SerializeField]
    private TextMeshProUGUI text;

    void Start()
    {
        relationManager.OnUpdate.AddListener(UpdateVisuals);
    }

    void UpdateVisuals()
    {
        text.text = relationManager.relationSetting.ToString();
    }

    public void SetNextGizmoRelation()
    {
        relationManager.SwitchToNextRelationSetting();
    }
}
