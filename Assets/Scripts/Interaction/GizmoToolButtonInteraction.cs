using System;
using Assets.Scripts.System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Assets.Scripts.Interaction
{
    public class GizmoToolButtonInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Dependencies
        private GizmoToolSystem gizmoToolSystem;

        [Inject]
        private void Construct(GizmoToolSystem gizmoToolSystem)
        {
            this.gizmoToolSystem = gizmoToolSystem;
        }


        public enum SettingToSwitch
        {
            GizmoToolModeTranslate,
            GizmoToolModeRotate,
            GizmoToolModeScale,
            GizmoToolRelationLocal,
            GizmoToolRelationGlobal,
            GizmoToolSnappingToggle
        }

        public void ToolButtonPressed(SettingToSwitch whichButton)
        {
            switch (whichButton)
            {
                case SettingToSwitch.GizmoToolModeTranslate:
                    gizmoToolSystem.gizmoToolMode = GizmoToolSystem.ToolMode.Translate;
                    break;
                case SettingToSwitch.GizmoToolModeRotate:
                    gizmoToolSystem.gizmoToolMode = GizmoToolSystem.ToolMode.Rotate;
                    break;
                case SettingToSwitch.GizmoToolModeScale:
                    gizmoToolSystem.gizmoToolMode = GizmoToolSystem.ToolMode.Scale;
                    break;
                case SettingToSwitch.GizmoToolRelationLocal:
                    gizmoToolSystem.gizmoToolContext = GizmoToolSystem.ToolContext.Local;
                    break;
                case SettingToSwitch.GizmoToolRelationGlobal:
                    gizmoToolSystem.gizmoToolContext = GizmoToolSystem.ToolContext.Global;
                    break;
                case SettingToSwitch.GizmoToolSnappingToggle:
                    gizmoToolSystem.isToolSnapping = !gizmoToolSystem.isToolSnapping;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(whichButton), whichButton, null);
            }
        }

        // Mouse hovering
        private bool isMouseOverButtons = false;

        public bool IsMouseOverButtons()
        {
            return isMouseOverButtons;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isMouseOverButtons = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isMouseOverButtons = false;
        }
    }
}