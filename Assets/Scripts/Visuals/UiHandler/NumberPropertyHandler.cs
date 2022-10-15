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

        public void SetActions(UiBuilder.UiPropertyActions<float> actions)
        {
            numberInput.SetActions(actions.OnChange, actions.OnSubmit, actions.OnAbort);
        }
    }
}
