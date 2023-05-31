using TMPro;
using UnityEngine;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class Vector3PropertyHandler : MonoBehaviour, IUpdateValue
    {
        [SerializeField]
        public TextMeshProUGUI propertyNameText;

        [SerializeField]
        public NumberInputHandler numberInputX;

        [SerializeField]
        public NumberInputHandler numberInputY;

        [SerializeField]
        public NumberInputHandler numberInputZ;


        private ValueBindStrategy<Vector3> bindStrategy;
        private Vector3? lastValue;

        public void ResetActions()
        {
            numberInputX.ResetActions();
            numberInputY.ResetActions();
            numberInputZ.ResetActions();
        }


        public void UpdateValue()
        {
            var currentValue = bindStrategy?.value();

            if (lastValue != currentValue)
            {
                numberInputX.SetCurrentNumber(currentValue?.x ?? 0);
                numberInputY.SetCurrentNumber(currentValue?.y ?? 0);
                numberInputZ.SetCurrentNumber(currentValue?.z ?? 0);

                lastValue = currentValue;
            }
        }

        public void Setup(ValueBindStrategy<Vector3> valueBindStrategy)
        {
            ResetActions();

            bindStrategy = valueBindStrategy;

            UpdateValue();

            ActionSetupHelper(numberInputX, valueBindStrategy);
            ActionSetupHelper(numberInputY, valueBindStrategy);
            ActionSetupHelper(numberInputZ, valueBindStrategy);
        }

        public void ActionSetupHelper(NumberInputHandler numberInputHandler, ValueBindStrategy<Vector3> valueBindStrategy)
        {
            numberInputHandler.SetActions(
                onChange: _ => { valueBindStrategy.onValueChanged?.Invoke(GetInputValue()); },
                onInvalid: _ => { valueBindStrategy.onErrorChanged?.Invoke(GetInputStrings()); },
                onSubmit: _ => { valueBindStrategy.onValueSubmitted?.Invoke(GetInputValue()); },
                onAbort: _ =>
                {
                    lastValue = null;
                    valueBindStrategy.onErrorSubmitted?.Invoke(GetInputStrings());
                    UpdateValue();
                });
        }

        public Vector3 GetInputValue()
        {
            return new Vector3(numberInputX.GetCurrentNumber() ?? 0, numberInputY.GetCurrentNumber() ?? 0, numberInputZ.GetCurrentNumber() ?? 0);
        }

        public string[] GetInputStrings()
        {
            return new[] {numberInputX.TextInputHandler.GetCurrentText(), numberInputY.TextInputHandler.GetCurrentText(), numberInputZ.TextInputHandler.GetCurrentText()};
        }
    }
}
