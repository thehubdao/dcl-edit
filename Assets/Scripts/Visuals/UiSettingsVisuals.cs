using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using UnityEditor.SearchService;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class UiSettingsVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        [SerializeField]
        private GameObject _content;

        // Dependencies
        private EditorEvents _editorEvents;
        private SettingsSystem _settingsSystem;
        private UiBuilder.Factory _uiBuilderFactory;

        [Inject]
        private void Construct(EditorEvents editorEvents, SettingsSystem settingsSystem, UiBuilder.Factory uiBuilderFactory)
        {
            _editorEvents = editorEvents;
            _settingsSystem = settingsSystem;
            _uiBuilderFactory = uiBuilderFactory;
        }

        public void SetupSceneEventListeners()
        {
            _editorEvents.onSettingsChangedEvent += UpdateVisuals;
            StartCoroutine(DelayedUpdate());
        }

        IEnumerator DelayedUpdate() // There are some problems with Zenject, when using the UiBuilder in the first frame
        {
            yield return null;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            var uiBuilder = _uiBuilderFactory.Create();

            foreach (var settingsPair in _settingsSystem.ShownSettings)
            {
                var categoryBuilder = _uiBuilderFactory.Create();

                categoryBuilder.PanelHeader(settingsPair.Key);

                foreach (var setting in settingsPair.Value)
                {
                    switch ((setting.stage, setting.type))
                    {
                        case (SettingsSystem.SettingStage.User, SettingsSystem.SettingType.String):
                        {
                            var concreteSetting = (SettingsSystem.StringUserSetting) setting;
                            var actions = new UiBuilder.UiPropertyActions<string>
                            {
                                OnChange = _ => { },
                                OnAbort = _ => { },
                                OnSubmit = value => { concreteSetting.Set(value); }
                            };
                            categoryBuilder.StringPropertyInput(concreteSetting.name, "", concreteSetting.Get(), actions);
                            break;
                        }
                        case (SettingsSystem.SettingStage.User, SettingsSystem.SettingType.Float):
                        {
                            var concreteSetting = (SettingsSystem.FloatUserSetting) setting;
                            var actions = new UiBuilder.UiPropertyActions<float>
                            {
                                OnChange = _ => { },
                                OnAbort = _ => { },
                                OnSubmit = value => { concreteSetting.Set(value); }
                            };
                            categoryBuilder.NumberPropertyInput(concreteSetting.name, "", concreteSetting.Get(), actions);
                            break;
                        }
                        case (SettingsSystem.SettingStage.User, SettingsSystem.SettingType.Integer):
                        {
                            var concreteSetting = (SettingsSystem.IntUserSetting) setting;
                            var actions = new UiBuilder.UiPropertyActions<float>
                            {
                                OnChange = _ => { },
                                OnAbort = _ => { },
                                OnSubmit = value => { concreteSetting.Set((int) value); }
                            };
                            categoryBuilder.NumberPropertyInput(concreteSetting.name, "", concreteSetting.Get(), actions);
                            break;
                        }
                        case (SettingsSystem.SettingStage.Project, SettingsSystem.SettingType.String):
                        {
                            var concreteSetting = (SettingsSystem.StringProjectSetting) setting;
                            var actions = new UiBuilder.UiPropertyActions<string>
                            {
                                OnChange = _ => { },
                                OnAbort = _ => { },
                                OnSubmit = value => { concreteSetting.Set(value); }
                            };
                            categoryBuilder.StringPropertyInput(concreteSetting.name, "", concreteSetting.Get(), actions);
                            break;
                        }
                        case (SettingsSystem.SettingStage.Project, SettingsSystem.SettingType.Vector3):
                        {
                            var concreteSetting = (SettingsSystem.Vec3ProjectSetting) setting;
                            var actions = new UiBuilder.UiPropertyActions<Vector3>
                            {
                                OnChange = _ => { },
                                OnAbort = _ => { },
                                OnSubmit = value => { concreteSetting.Set(value); }
                            };
                            string[] placeholders = {"", "", ""};
                            categoryBuilder.Vector3PropertyInput(concreteSetting.name, placeholders, concreteSetting.Get(), actions);
                            break;
                        }
                        case (SettingsSystem.SettingStage.Scene, SettingsSystem.SettingType.Vector3):
                        {
                            var concreteSetting = (SettingsSystem.Vec3SceneSetting) setting;
                            var actions = new UiBuilder.UiPropertyActions<Vector3>
                            {
                                OnChange = _ => { },
                                OnAbort = _ => { },
                                OnSubmit = value => { concreteSetting.Set(value); }
                            };
                            string[] placeholders = {"", "", ""};
                            categoryBuilder.Vector3PropertyInput(concreteSetting.name, placeholders, concreteSetting.Get(), actions);
                            break;
                        }
                        default:
                            throw new Exception($"No Setting Type available for stage {setting.stage} and type {setting.type}");
                    }
                }

                uiBuilder.Panel(categoryBuilder);
            }

            uiBuilder.ClearAndMake(_content);
        }
    }
}