using Assets.Scripts.EditorState;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visuals
{
    public class SnackbarItemHandler : MonoBehaviour
    {
        public Image background;
        public Image icon;
        public TextMeshProUGUI text;
        public Button button;
        public SnackbarState.Data data;
    }
}