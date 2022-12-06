using Assets.Scripts.Visuals.UiBuilder;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Visuals.PropertyHandler
{
    public class NumberPropertyHandler : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI propertyNameText;
        
        [SerializeField]
        public NumberInputHandler numberInput;

        public void SetActions(StringPropertyAtom.UiPropertyActions<float> actions)
        {
            numberInput.SetActions(actions.OnChange, actions.OnSubmit, actions.OnAbort);
        }

        public void ResetActions()
        {
            numberInput.ResetActions();
        }
    }
}
