using System;
using Assets.Scripts.Interaction;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class SceneViewButtonHandler : MonoBehaviour
    {
        public GizmoToolButtonInteraction.SettingToSwitch settingToSwitch;

        [SerializeField]
        private Button button;


        public void SetInteractability(bool shouldBeInteractable)
        {
            button.interactable = shouldBeInteractable;
        }

        public void SetButtonAction(Action action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action());
        }
    }
}
