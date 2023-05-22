using System;
using JetBrains.Annotations;
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
        public TMP_ColorGradient NormalColorGradient;

        [SerializeField]
        public TMP_ColorGradient PrimaryColorGradient;

        [SerializeField]
        public TMP_ColorGradient SecondaryColorGradient;

        [SerializeField]
        public TMP_ColorGradient DisabledColorGradient;

        [Header("Scene References")]
        [SerializeField]
        public TextMeshProUGUI TextComponent;

        [CanBeNull]
        private SetValueStrategy<string> valueStrategyInternal;

        public void SetTextValueStrategy(SetValueStrategy<string> strategy)
        {
            if (valueStrategyInternal != null)
            {
                valueStrategyInternal.applyValue = null;
            }

            strategy.applyValue = s => TextComponent.text = s;

            valueStrategyInternal = strategy;
        }

        public TextStyle textStyle
        {
            get
            {
                if (TextComponent.colorGradientPreset == NormalColorGradient)
                {
                    return TextStyle.Normal;
                }

                if (TextComponent.colorGradientPreset == PrimaryColorGradient)
                {
                    return TextStyle.PrimarySelection;
                }

                if (TextComponent.colorGradientPreset == SecondaryColorGradient)
                {
                    return TextStyle.SecondarySelection;
                }

                if (TextComponent.colorGradientPreset == DisabledColorGradient)
                {
                    return TextStyle.Disabled;
                }

                throw new ArgumentOutOfRangeException();
            }

            set => TextComponent.colorGradientPreset = value switch
            {
                TextStyle.Normal => NormalColorGradient,
                TextStyle.PrimarySelection => PrimaryColorGradient,
                TextStyle.SecondarySelection => SecondaryColorGradient,
                TextStyle.Disabled => DisabledColorGradient,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }
}
