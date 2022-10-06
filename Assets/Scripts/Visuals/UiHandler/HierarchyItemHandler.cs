using System;
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

        public struct UiHierarchyItemActions
        {
            public Action OnArrowClick;
            public Action OnNameClick;
        }

        public UiHierarchyItemActions actions
        {
            set
            {
                Arrow.GetComponent<Toggle>().onValueChanged.AddListener(_ => value.OnArrowClick());
                Text.GetComponent<Button>().onClick.AddListener(() => value.OnNameClick());
            }
        }
    }
}
