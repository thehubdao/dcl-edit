using Assets.Scripts.Visuals.NewUiBuilder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class BooleanPropertyHandler : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI PropertyNameText;

        [SerializeField]
        public Toggle CheckBoxInput;

        public void SetActions(UiBuilder.UiPropertyActions<bool> actions)
        {
            CheckBoxInput.onValueChanged.RemoveAllListeners();

            CheckBoxInput.onValueChanged.AddListener(value => actions.OnSubmit(value));
        }

        public void SetActions(StringPropertyAtom.UiPropertyActions<bool> actions)
        {
            CheckBoxInput.onValueChanged.RemoveAllListeners();

            CheckBoxInput.onValueChanged.AddListener(value => actions.OnSubmit(value));
        }
    }
}
