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
        private RectTransform Arrow;

        [SerializeField]
        private RectTransform ArrowContainer;

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
                Arrow.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
                Text.GetComponent<Button>().onClick.RemoveAllListeners();

                Arrow.GetComponent<Toggle>().onValueChanged.AddListener(_ => value.OnArrowClick());
                Text.GetComponent<Button>().onClick.AddListener(() => value.OnNameClick());
            }
        }

        public bool isExpanded
        {
            set
            {
                if (value)
                {
                    ArrowContainer.rotation = Quaternion.Euler(0, 0, 0);
                    ArrowContainer.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    ArrowContainer.rotation = Quaternion.Euler(0, 0, 90);
                    ArrowContainer.localScale = new Vector3(-1, 1, 1);
                }
            }
        }

        public bool showArrow
        {
            set => ArrowContainer.gameObject.SetActive(value);
        }
    }
}