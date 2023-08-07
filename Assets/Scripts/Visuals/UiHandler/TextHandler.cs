using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class TextHandler : MonoBehaviour
    {
        public enum TextStyle
        {
            Normal,
            PrimarySelection,
            SecondarySelection,
            Disabled
        }

        [SerializeField]
        public TMP_ColorGradient normalColorGradient;

        [SerializeField]
        public TMP_ColorGradient primaryColorGradient;

        [SerializeField]
        public TMP_ColorGradient secondaryColorGradient;

        [SerializeField]
        public TMP_ColorGradient disabledColorGradient;

        [Header("Scene References")]
        [SerializeField]
        public TextMeshProUGUI textComponent;

        public Action<string> onLinkClicked;
        
        public string text
        {
            get => textComponent.text;
            set => textComponent.text = value;
        }

        public TextStyle textStyle
        {
            get
            {
                if (textComponent.colorGradientPreset == normalColorGradient)
                {
                    return TextStyle.Normal;
                }

                if (textComponent.colorGradientPreset == primaryColorGradient)
                {
                    return TextStyle.PrimarySelection;
                }

                if (textComponent.colorGradientPreset == secondaryColorGradient)
                {
                    return TextStyle.SecondarySelection;
                }

                if (textComponent.colorGradientPreset == disabledColorGradient)
                {
                    return TextStyle.Disabled;
                }

                throw new ArgumentOutOfRangeException();
            }

            set => textComponent.colorGradientPreset = value switch
            {
                TextStyle.Normal => normalColorGradient,
                TextStyle.PrimarySelection => primaryColorGradient,
                TextStyle.SecondarySelection => secondaryColorGradient,
                TextStyle.Disabled => disabledColorGradient,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
        
        //TODO onLinkClicked sometimes null!
        private void LateUpdate()
        {
            if (!textComponent.TryGetLinkID(out var selectedLinkID))
            {
                return;
            }
            
            Debug.Log("Link Clicked!");
            onLinkClicked?.Invoke(selectedLinkID);
        }
        
    }
}
