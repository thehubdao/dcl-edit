using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class HierarchyItemHandler : MonoBehaviour
    {
        [SerializeField]
        public TextHandler Text;

        [SerializeField]
        public RectTransform Arrow;

        [SerializeField]
        public RectTransform Indent;
    }
}
