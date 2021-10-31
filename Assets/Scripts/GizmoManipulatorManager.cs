using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GizmoManipulatorManager : MonoBehaviour
{
    private Manipulator _currentManipulator = Manipulator.Translate;

    [SerializeField]
    private TextMeshProUGUI _currentManipulatorText;

    public enum Manipulator
    {
        Translate,
        Rotate,
        Scale
    }

    public Manipulator CurrentManipulator
    {
        get => _currentManipulator;
        set
        {
            _currentManipulator = value;
            var mps = GetComponentsInChildren<ManipulatorParent>();
            foreach (var mp in mps)
            {
                mp.UpdateVisuals();
            }

            _currentManipulatorText.text = _currentManipulator.ToString();
        }
    }

    public void SwitchToNextManipulator()
    {
        CurrentManipulator = CurrentManipulator.Next();
    }

}
