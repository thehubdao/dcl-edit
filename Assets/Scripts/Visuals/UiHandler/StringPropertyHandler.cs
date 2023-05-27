using System;
using Assets.Scripts.Events;
using Assets.Scripts.Visuals.UiBuilder;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals.PropertyHandler
{
    public class StringPropertyHandler : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI propertyNameText;
        
        [SerializeField]
        public TextInputHandler stringInput;

        [Inject]
        private void Construct(EditorEvents editorEvents)
        {
            editorEvents.onValueChangedEvent += UpdateValue;
        }

        [CanBeNull]
        private ValueBindStrategy<string> bindStrategy;

        [CanBeNull]
        private string lastValue = null;

        private void UpdateValue()
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

            Action<string> onChange = bindStrategy.onValueChanged;
            Action<string> onSubmit = bindStrategy.onValueSubmitted;
            Action<string> onAbort = s => bindStrategy.onErrorSubmitted?.Invoke(new[] {s});

            stringInput.SetActions(onChange, onSubmit, onAbort);

            this.bindStrategy = bindStrategy;

            UpdateValue();
        }

        public void ResetActions()
        {
            stringInput.ResetActions();
        }
    }
}
