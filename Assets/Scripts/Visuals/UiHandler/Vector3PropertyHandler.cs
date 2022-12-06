using Assets.Scripts.Visuals.UiBuilder;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Visuals.UiHandler
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
            SetActionsHelper(numberInputX, actions);
            SetActionsHelper(numberInputY, actions);
            SetActionsHelper(numberInputZ, actions);
        }

        private Vector3? GetCurrentValue()
        {
            var xInput = numberInputX.GetCurrentNumber();
            var yInput = numberInputY.GetCurrentNumber();
            var zInput = numberInputZ.GetCurrentNumber();
            if (xInput == null  || yInput == null || zInput == null)
            {
                return null;
            }
            return new Vector3(xInput.Value, yInput.Value, zInput.Value);
        }

        private void SetActionsHelper(NumberInputHandler numberInput, StringPropertyAtom.UiPropertyActions<Vector3> actions)
        {
            numberInput.SetActions(
                _ =>
                {
                    var currentValue = GetCurrentValue();
                    if (currentValue == null)
                    {
                        actions.OnInvalid?.Invoke();
                    }
                    else
                    {
                        actions.OnChange(currentValue.Value);
                    }
                },
                () =>
                {
                    actions.OnInvalid?.Invoke();
                },
                _ =>
                {
                    var currentValue = GetCurrentValue();
                    if (currentValue == null)
                    {
                        actions.OnAbort(Vector3.zero);
                    }
                    else
                    {
                        actions.OnSubmit(currentValue.Value);
                    }
                },
                _ =>
                {
                    var currentValue = GetCurrentValue();
                    if (currentValue == null)
                    {
                        actions.OnAbort(Vector3.zero);
                    }
                    else
                    {
                        actions.OnAbort(currentValue.Value);
                    }
                });
        }
    }
}
