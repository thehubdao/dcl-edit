using Assets.Scripts.Visuals.NewUiBuilder;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Visuals.PropertyHandler
{
    public class StringPropertyHandler : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI propertyNameText;
        
        [SerializeField]
        public TextInputHandler stringInput;

        public void SetActions(UiBuilder.UiPropertyActions<string> actions)
        {
            stringInput.SetActions(actions.OnChange,actions.OnSubmit,actions.OnAbort);
        }

        public void SetActions(StringPropertyAtom.UiPropertyActions<string> actions)
        {
            stringInput.SetActions(actions.OnChange, actions.OnSubmit, actions.OnAbort);
        }

        public void ResetActions()
        {
            stringInput.ResetActions();
        }
    }
}
