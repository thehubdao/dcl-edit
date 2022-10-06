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
        }

        [SerializeField]
        public TMP_ColorGradient NormalColorGradient;

        [SerializeField]
        public TMP_ColorGradient PrimaryColorGradient;

        [SerializeField]
        public TMP_ColorGradient SecondaryColorGradient;

        [Header("Scene References")]
        [SerializeField]
        public TextMeshProUGUI TextComponent;

        public string text
        {
            get => TextComponent.text;
            set => TextComponent.text = value;
        }

        public TextStyle textStyle
        {
            set => TextComponent.colorGradientPreset = value switch
            {
                TextStyle.Normal => NormalColorGradient,
                TextStyle.PrimarySelection => PrimaryColorGradient,
                TextStyle.SecondarySelection => SecondaryColorGradient,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }
}
