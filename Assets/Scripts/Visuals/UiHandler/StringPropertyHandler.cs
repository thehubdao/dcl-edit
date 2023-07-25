using System;
using Assets.Scripts.Events;
using Assets.Scripts.Visuals.UiBuilder;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals.PropertyHandler
{
    public class StringPropertyHandler : MonoBehaviour, IUpdateValue
    {
        [SerializeField]
        public TextMeshProUGUI propertyNameText;
        
        [SerializeField]
        public TextInputHandler stringInput;


        [CanBeNull]
        private ValueBindStrategy<string> bindStrategy;

        [CanBeNull]
        private string lastValue = null;

        public void UpdateValue()
        {
            var currentValue = bindStrategy?.value();

            if (lastValue != currentValue)
            {
                stringInput.SetCurrentText(currentValue);
                lastValue = currentValue;
            }
        }

        public void Setup(ValueBindStrategy<string> bindStrategy)
        {
            ResetActions();

            this.bindStrategy = bindStrategy;

            UpdateValue();

            Action<string> onChange = bindStrategy.onValueChanged;
            Action<string> onSubmit = bindStrategy.onValueSubmitted;
            Action<string> onAbort = s => bindStrategy.onErrorSubmitted?.Invoke(new[] {s});

            stringInput.SetActions(onChange, onSubmit, onAbort);
        }

        public void ResetActions()
        {
            stringInput.ResetActions();
        }
    }
}
