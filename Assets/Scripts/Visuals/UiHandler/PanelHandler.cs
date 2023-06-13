using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class PanelHandler : MonoBehaviour
    {
        public GameObject content;

        public enum LayoutDirection
        {
            Horizontal,
            Vertical
        }

        public void SetLayoutDirection(LayoutDirection value, TextAnchor anchor)
        {
            if (IsLayoutGroupCorrect(value)) return;

            RemoveLayoutGroups();

            HorizontalOrVerticalLayoutGroup layoutGroup;

            switch (value)
            {
                case LayoutDirection.Horizontal:
                    layoutGroup = content.AddComponent<HorizontalLayoutGroup>();
                    layoutGroup.childForceExpandHeight = false;
                    layoutGroup.childForceExpandWidth = false;
                    break;
                case LayoutDirection.Vertical:
                    layoutGroup = content.AddComponent<VerticalLayoutGroup>();
                    layoutGroup.childForceExpandHeight = false;
                    layoutGroup.childForceExpandWidth = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            layoutGroup.childAlignment = anchor;
        }

        private bool IsLayoutGroupCorrect(LayoutDirection value)
        {
            switch (value)
            {
                case LayoutDirection.Horizontal:
                    return content.TryGetComponent(out HorizontalLayoutGroup _);
                case LayoutDirection.Vertical:
                    return content.TryGetComponent(out VerticalLayoutGroup _);
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        private void RemoveLayoutGroups()
        {
            if (content.TryGetComponent(out HorizontalLayoutGroup hlg))
            {
                Destroy(hlg);
            }

            if (content.TryGetComponent(out VerticalLayoutGroup vlg))
            {
                Destroy(vlg);
            }
        }
    }
}
