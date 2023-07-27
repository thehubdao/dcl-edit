using System.Linq;
using Assets.Scripts.EditorState;
using Zenject;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
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
            ResetScrollBars();
            
            var versionsPanelData = UiBuilder.NewPanelData();
            versionsPanelData.layoutDirection = PanelHandler.LayoutDirection.Vertical;

            var orderedChangeLogs = changeLogState.ChangeLog.OrderByDescending(l => l.version);

            var defaultDescriptionPanelData = UiBuilder.NewPanelData();
            defaultDescriptionPanelData.AddText(orderedChangeLogs.First().details);

            AddButtons(orderedChangeLogs, versionsPanelData);
            
            uiBuilderVersions.Update(versionsPanelData);
            uiBuilderDescription.Update(defaultDescriptionPanelData);
        }

        private void AddButtons(IOrderedEnumerable<ChangeLogState.ChangeLogStructure> orderedChangeLogs,
            PanelAtom.Data versionsPanelData)
        {
            foreach (var changeLog in orderedChangeLogs)
            {
                //TODO access button to change layout (and color?)
                versionsPanelData.AddButton(changeLog.version, go =>
                {
                    var button = go.GetComponent<Button>();
                    var colors = button.colors;
                    colors.selectedColor = Color.blue;
                    button.colors = colors;
                    button.Select();

                    var descriptionPanelData = UiBuilder.NewPanelData();
                    descriptionPanelData.AddText(changeLog.details);
                    scrollRectDescription.verticalScrollbar.value = 1;

                    uiBuilderDescription.Update(descriptionPanelData);
                });
            }
        }

        private void ResetScrollBars()
        {
            //TODO not working
            scrollRectDescription.verticalScrollbar.value = 1;
            scrollRectVersions.verticalScrollbar.value = 1;
        }
    }
}
