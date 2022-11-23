using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class TextHandler : MonoBehaviour
    {
        public enum TextColor
        {
            Normal,
            PrimarySelection,
            SecondarySelection
        }

        public struct TextStyle
        {
            public HorizontalAlignmentOptions horizontalAlignment;
            public VerticalAlignmentOptions verticalAlignment;
            public TextColor color;

            // Text style presets
            public static TextStyle NormalTextStyle => new TextStyle
            {
                horizontalAlignment = HorizontalAlignmentOptions.Left,
                verticalAlignment = VerticalAlignmentOptions.Middle,
                color = TextColor.Normal
            };

            public static TextStyle PrimarySelectionTextStyle => new TextStyle
            {
                horizontalAlignment = HorizontalAlignmentOptions.Left,
                verticalAlignment = VerticalAlignmentOptions.Middle,
                color = TextColor.PrimarySelection
            };

            public static TextStyle SecondarySelectionTextStyle => new TextStyle
            {
                horizontalAlignment = HorizontalAlignmentOptions.Left,
                verticalAlignment = VerticalAlignmentOptions.Middle,
                color = TextColor.SecondarySelection
            };
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
            set
            {
                TextComponent.horizontalAlignment = value.horizontalAlignment;
                TextComponent.verticalAlignment = value.verticalAlignment;
                TextComponent.colorGradientPreset = value.color switch
                {
                    TextColor.Normal => NormalColorGradient,
                    TextColor.PrimarySelection => PrimaryColorGradient,
                    TextColor.SecondarySelection => SecondaryColorGradient,
                    _ => throw new NotImplementedException(),
                };
            }
        }
    }
}
