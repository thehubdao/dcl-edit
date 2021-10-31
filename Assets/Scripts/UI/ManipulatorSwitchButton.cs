using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ManipulatorSwitchButton : MonoBehaviour
{
    [SerializeField]
    private GizmoManipulatorManager manipulatorManager;

    [SerializeField]
    private TextMeshProUGUI text;

    void Start()
    {
        manipulatorManager.OnUpdate.AddListener(UpdateVisuals);
    }

    void UpdateVisuals()
    {
        text.text = manipulatorManager.CurrentManipulator.ToString();
    }

    public void SetNextManipulator()
    {
        manipulatorManager.SwitchToNextManipulator();
    }
}
