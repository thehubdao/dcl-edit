using Assets.Scripts.Visuals.UiBuilder;
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

        public void SetActions(StringPropertyAtom.UiPropertyActions<string> actions)
        {
            stringInput.SetActions(actions.OnChange, actions.OnSubmit, s => actions.OnAbort?.Invoke(new[] {s}));
        }

        public void ResetActions()
        {
            stringInput.ResetActions();
        }
    }
}
