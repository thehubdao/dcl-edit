using System.Linq;
using Assets.Scripts.EditorState;
using Zenject;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Visuals
{
    public class ChangeLogVisuals : MonoBehaviour
    {
        [SerializeField]
        private GameObject contentVersions;

        [SerializeField]
        private GameObject contentDescription;

        [SerializeField]
        private ScrollRect scrollRectVersions;

        [SerializeField]
        private ScrollRect scrollRectDescription;

        private UiBuilder uiBuilderVersions;
        private UiBuilder uiBuilderDescription;
        private ChangeLogState changeLogState;

        [Inject]
        void Construct(
            UiBuilder.Factory uiBuilderFactory,
            ChangeLogState changeLogState)
        {
            uiBuilderVersions = uiBuilderFactory.Create(contentVersions);
            uiBuilderDescription = uiBuilderFactory.Create(contentDescription);
            this.changeLogState = changeLogState;
        }

        public void AddChangeLog()
        {
            var versionsPanelData = UiBuilder.NewPanelData();
            versionsPanelData.layoutDirection = PanelHandler.LayoutDirection.Vertical;

            var orderedChangeLogs = changeLogState.ChangeLog.OrderByDescending(l => l.version);

            var defaultDescriptionPanelData = UiBuilder.NewPanelData();
            var descriptionText = defaultDescriptionPanelData.AddText(orderedChangeLogs.First().details);
            descriptionText.textAlignment = TextAlignmentOptions.MidlineLeft;

            AddButtons(orderedChangeLogs, versionsPanelData);

            uiBuilderVersions.Update(versionsPanelData);
            uiBuilderDescription.Update(defaultDescriptionPanelData);
        }

        private void AddButtons(IOrderedEnumerable<ChangeLogState.ChangeLogStructure> orderedChangeLogs,
            PanelAtom.Data versionsPanelData)
        {
            var orderedChangeLogsList = orderedChangeLogs.ToList();
            
            foreach (var changeLog in orderedChangeLogsList)
            {
                var butt = versionsPanelData.AddButton(changeLog.version, go =>
                {
                    var button = go.GetComponent<Button>();
                    button.Select();

                    var descriptionPanelData = UiBuilder.NewPanelData();
                    
                    var descriptionText = descriptionPanelData.AddText(changeLog.details);
                    descriptionText.textAlignment = TextAlignmentOptions.MidlineLeft;
                    
                    scrollRectDescription.verticalScrollbar.value = 1;

                    uiBuilderDescription.Update(descriptionPanelData);
                });

                butt.textAnchor = TextAnchor.UpperCenter;
                butt.expandHorInLayout = true;

                var newColors = ColorBlock.defaultColorBlock;
                newColors.selectedColor = Color.magenta;
                butt.customColors = newColors;

                if (ReferenceEquals(changeLog, orderedChangeLogsList[0]))
                {
                    butt.isSelected = true;
                }
            }
        }

        public void ResetScrollBars()
        {
            scrollRectDescription.verticalScrollbar.value = 1;
            scrollRectVersions.verticalScrollbar.value = 1;
        }
    }
}