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

        public void SetLayoutDirection(LayoutDirection value, bool useFullWidth = true)
        {
            if (IsLayoutGroupCorrect(value)) return;

            RemoveLayoutGroups();

            HorizontalOrVerticalLayoutGroup layoutGroup;

            switch (value)
            {
                case LayoutDirection.Horizontal:
                    layoutGroup = content.AddComponent<HorizontalLayoutGroup>();
                    break;
                case LayoutDirection.Vertical:
                    layoutGroup = content.AddComponent<VerticalLayoutGroup>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = useFullWidth;
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
