using Assets.Scripts.Visuals.UiBuilder;
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

        public void SetActions(StringPropertyAtom.UiPropertyActions<bool> actions)
        {
            ResetActions();

            CheckBoxInput.onValueChanged.AddListener(value => actions.OnSubmit(value));
        }

        public void ResetActions()
        {
            CheckBoxInput.onValueChanged.RemoveAllListeners();
        }
    }
}
