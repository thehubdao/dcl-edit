using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class HierarchyItemHandler : MonoBehaviour
    {
        [SerializeField]
        public TextHandler text;

        [SerializeField]
        private RectTransform arrow;

        [SerializeField]
        private RectTransform arrowContainer;

        [SerializeField]
        public RectTransform indent;

        [SerializeField]
        public RightClickHandler rightClickHandler;

        public bool primarySelection { get; set; }

        public struct UiHierarchyItemActions
        {
            [CanBeNull]
            public Action onArrowClick;

            [CanBeNull]
            public Action onNameClick;
        }

        public UiHierarchyItemActions actions
        {
            set
            {
                arrow.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
                text.GetComponent<Button>().onClick.RemoveAllListeners();

                arrow.GetComponent<Toggle>().onValueChanged.AddListener(_ => value.onArrowClick?.Invoke());
                text.GetComponent<Button>().onClick.AddListener(() => value.onNameClick?.Invoke());
            }
        }

        public bool isExpanded
        {
            set
            {
                if (value)
                {
                    arrowContainer.rotation = Quaternion.Euler(0, 0, 0);
                    arrowContainer.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    arrowContainer.rotation = Quaternion.Euler(0, 0, 90);
                    arrowContainer.localScale = new Vector3(-1, 1, 1);
                }
            }
        }

        public bool showArrow
        {
            set => arrowContainer.gameObject.SetActive(value);
        }
    }
}