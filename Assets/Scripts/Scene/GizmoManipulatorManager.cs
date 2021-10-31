using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GizmoManipulatorManager : MonoBehaviour
{
    private Manipulator _currentManipulator = Manipulator.Translate;

    [SerializeField]
    //private TextMeshProUGUI _currentManipulatorText;

    public UnityEvent OnUpdate;

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
            OnUpdate.Invoke();
            //var mps = GetComponentsInChildren<ManipulatorParent>();
            //foreach (var mp in mps)
            //{
            //    mp.SetDirty();
            //}
            //
            //_currentManipulatorText.text = _currentManipulator.ToString();
        }
    }

    public void SwitchToNextManipulator()
    {
        CurrentManipulator = CurrentManipulator.Next();
    }

}
