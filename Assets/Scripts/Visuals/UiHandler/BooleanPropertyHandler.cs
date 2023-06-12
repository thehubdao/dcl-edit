using Assets.Scripts.Visuals.UiBuilder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class BooleanPropertyHandler : MonoBehaviour, IUpdateValue
    {
        [SerializeField]
        public TextMeshProUGUI PropertyNameText;

        [SerializeField]
        public Toggle CheckBoxInput;

        private ValueBindStrategy<bool> bindStrategy;

        public void SetActions(StringPropertyAtom.UiPropertyActions<bool> actions)
        {
            ResetActions();

            CheckBoxInput.onValueChanged.AddListener(value => actions.OnSubmit(value));
        }

        public void ResetActions()
        {
            CheckBoxInput.onValueChanged.RemoveAllListeners();
        }

        public void Setup(ValueBindStrategy<bool> valueBindStrategy)
        {
            ResetActions();

            bindStrategy = valueBindStrategy;

            UpdateValue();

            CheckBoxInput.onValueChanged.AddListener(value => bindStrategy.onValueSubmitted?.Invoke(value));
        }

        public void UpdateValue()
        {
            CheckBoxInput.isOn = bindStrategy.value();
        }
    }
}
