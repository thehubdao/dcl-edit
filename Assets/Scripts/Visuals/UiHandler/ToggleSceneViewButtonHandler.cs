using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class ToggleSceneViewButtonHandler : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private Color offColor;

        [SerializeField]
        private Color onColor;

        public void SetOnOff(bool isOn)
        {
            var buttonColors = button.colors;
            buttonColors.normalColor = isOn ? onColor : offColor;
            button.colors = buttonColors;
        }
    }
}
