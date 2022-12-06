using Assets.Scripts.Visuals.UiBuilder;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Visuals.PropertyHandler
{
    public class Vector3PropertyHandler : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI propertyNameText;

        [SerializeField]
        public NumberInputHandler numberInputX;

        [SerializeField]
        public NumberInputHandler numberInputY;

        [SerializeField]
        public NumberInputHandler numberInputZ;

        public void ResetActions()
        {
            numberInputX.ResetActions();
            numberInputY.ResetActions();
            numberInputZ.ResetActions();
        }

        public void SetActions(StringPropertyAtom.UiPropertyActions<Vector3> actions)
        {
            numberInputX.SetActions(
                _ => actions.OnChange(GetCurrentValue()),
                _ => actions.OnSubmit(GetCurrentValue()),
                _ => actions.OnAbort(GetCurrentValue())
            );

            numberInputY.SetActions(
                _ => actions.OnChange(GetCurrentValue()),
                _ => actions.OnSubmit(GetCurrentValue()),
                _ => actions.OnAbort(GetCurrentValue())
            );

            numberInputZ.SetActions(
                _ => actions.OnChange(GetCurrentValue()),
                _ => actions.OnSubmit(GetCurrentValue()),
                _ => actions.OnAbort(GetCurrentValue())
            );
        }

        public Vector3 GetCurrentValue()
        {
            return new Vector3(numberInputX.GetCurrentNumber(), numberInputY.GetCurrentNumber(), numberInputZ.GetCurrentNumber());
        }
    }
}
