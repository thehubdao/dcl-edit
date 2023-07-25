using System.Linq;
using Assets.Scripts.EditorState;
using Zenject;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using UnityEngine;

namespace Visuals
{
    public class ChangeLogVisuals: MonoBehaviour
    {
        [SerializeField]
        private GameObject changeLogVersions;
        [SerializeField]
        private GameObject changeLogDescription;
        
        private UiBuilder uiBuilderVersions;
        private UiBuilder uiBuilderDescription;
        private ChangeLogState changeLogState;

        [Inject]
        void Construct(
            UiBuilder.Factory uiBuilderFactory,
            ChangeLogState changeLogState)
        {
            uiBuilderVersions = uiBuilderFactory.Create(changeLogVersions);
            uiBuilderDescription = uiBuilderFactory.Create(changeLogDescription);
            this.changeLogState = changeLogState;
        }

        public void AddChangeLog()
        {
            var versionsPanelData = UiBuilder.NewPanelData();

            versionsPanelData.layoutDirection = PanelHandler.LayoutDirection.Vertical;

            var orderedChangeLogs = changeLogState.ChangeLog.OrderBy(l => l.version);
            
            foreach (var changeLog in orderedChangeLogs)
            {
                versionsPanelData.AddButton(changeLog.version, arg0 =>
                {
                    var descriptionPanelData = UiBuilder.NewPanelData();
                    descriptionPanelData.AddText(changeLog.details);
                    
                    uiBuilderDescription.Update(descriptionPanelData);
                });
            }

            uiBuilderVersions.Update(versionsPanelData);
        }
    }
}