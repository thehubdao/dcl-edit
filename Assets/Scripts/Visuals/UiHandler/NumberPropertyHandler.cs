using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Visuals.PropertyHandler
{
    public class NumberPropertyHandler : MonoBehaviour, IUpdateValue
    {
        [SerializeField]
        public TextMeshProUGUI propertyNameText;
        
        [SerializeField]
        public NumberInputHandler numberInput;


        [CanBeNull]
        private ValueBindStrategy<float> bindStrategy;

        private float? lastValue = null;

        public void UpdateValue()
        {
            var currentValue = bindStrategy?.value();

            if (lastValue != currentValue)
            {
                numberInput.SetCurrentNumber(currentValue ?? 0);
                lastValue = currentValue;
            }
        }

        public void Setup(ValueBindStrategy<float> bindStrategy)
        {
            ResetActions();

            this.bindStrategy = bindStrategy;

            UpdateValue();

            numberInput.SetActions(
                bindStrategy.onValueChanged,
                bindStrategy.onErrorChanged,
                bindStrategy.onValueSubmitted,
                value =>
                {
                    lastValue = null;
                    bindStrategy.onErrorSubmitted?.Invoke(value);
                    UpdateValue();
                });
        }

        public void ResetActions()
        {
            numberInput.ResetActions();
        }
    }
}
