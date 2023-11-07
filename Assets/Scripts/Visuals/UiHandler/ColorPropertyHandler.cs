using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visuals.PropertyHandler
{
    public class ColorPropertyHandler : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI propertyNameText;
        
        [SerializeField]
        public ButtonHandler colorPicker;
        public TextInputHandler stringInput;
        public Image colorImage;

        public void SetActions(ColorPropertyAtom.UiPropertyActions<Color> actions)
        {
            stringInput.SetActions(
                (value) =>
                {
                    Color colorValue;
                    if (ColorUtility.TryParseHtmlString("#" + value, out colorValue))
                    {
                        colorImage.color = colorValue;
                        actions.OnChange?.Invoke(colorValue);
                    }
                },
                (value) =>
                {
                    Color colorValue;
                    if (ColorUtility.TryParseHtmlString("#" + value, out colorValue))
                    {
                        colorImage.color = colorValue;
                        actions.OnSubmit?.Invoke(colorValue);
                    }
                },
                (value) =>
                {
                    Color colorValue = colorImage.color;
                    actions.OnAbort?.Invoke(colorValue);
                });
        }

        public void ResetActions()
        {
            stringInput.ResetActions();
        }
    }
}
