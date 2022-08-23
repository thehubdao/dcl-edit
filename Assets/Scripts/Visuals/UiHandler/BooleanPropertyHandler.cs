using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class BooleanPropertyHandler : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI PropertyNameText;
        
        [SerializeField]
        public Toggle CheckBoxInput;
    }
}
