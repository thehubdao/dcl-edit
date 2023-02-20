using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class UiSettingsVisuals : MonoBehaviour
    {
        [SerializeField]
        private GameObject content;

        // Dependencies
        private EditorEvents editorEvents;
        private SettingsSystem settingsSystem;
        private UiBuilder.UiBuilder uiBuilder;
        private UnityState unityState;
        private InputState inputState;

        [Inject]
        private void Construct(
            EditorEvents editorEvents,
            SettingsSystem settingsSystem,
            UiBuilder.UiBuilder.Factory uiBuilderFactory,
            UnityState unityState,
            InputState inputState)
        {
            this.editorEvents = editorEvents;
            this.settingsSystem = settingsSystem;
            this.uiBuilder = uiBuilderFactory.Create(content);
            this.unityState = unityState;
            this.inputState = inputState;

            SetupEventListeners();
        }

        public void SetupEventListeners()
        {
            editorEvents.onSettingsChangedEvent += SetDirty;
            unityState.StartCoroutine(DelayedUpdate()); // Unity state is guarantied to be available and active
        }

        IEnumerator DelayedUpdate() // There are some problems with Zenject, when using the UiBuilder in the first frame
        {
            yield return null;
            SetDirty();
        }

        private bool _dirty;

        void SetDirty()
        {
            _dirty = true;
        }

        void LateUpdate()
        {
            if (_dirty)
            {
                _dirty = false;
                UpdateVisuals();
            }
        }

        private void UpdateVisuals()
        {
            if (inputState.InState == InputState.InStateType.UiInput)
            {
                return;
            }

            var settingsPanel = new PanelAtom.Data();

            foreach (var settingsPair in settingsSystem.ShownSettings)
            {
                var categoryPanel = settingsPanel.AddPanelWithBorder();

                categoryPanel.AddPanelHeader(settingsPair.Key);

                foreach (var setting in settingsPair.Value)
                {
                    switch ((setting.stage, setting.type))
                    {
                        case (SettingsSystem.SettingStage.User, SettingsSystem.SettingType.String):
                        {
                            var concreteSetting = (SettingsSystem.StringUserSetting) setting;
                            var actions = new StringPropertyAtom.UiPropertyActions<string>
                            {
                                OnChange = _ => { },
                                OnAbort = _ => { },
                                OnSubmit = value => { concreteSetting.Set(value); }
                            };
                            categoryPanel.AddStringProperty(concreteSetting.name, "", concreteSetting.Get(), actions);
                            break;
                        }
                        case (SettingsSystem.SettingStage.User, SettingsSystem.SettingType.Float):
                        {
                            var concreteSetting = (SettingsSystem.FloatUserSetting) setting;
                            var actions = new StringPropertyAtom.UiPropertyActions<float>
                            {
                                OnChange = _ => { },
                                OnInvalid = () => { },
                                OnAbort = _ => { },
                                OnSubmit = value => { concreteSetting.Set(value); }
                            };
                            categoryPanel.AddNumberProperty(concreteSetting.name, "", concreteSetting.Get(), actions);
                            break;
                        }
                        case (SettingsSystem.SettingStage.User, SettingsSystem.SettingType.Integer):
                        {
                            var concreteSetting = (SettingsSystem.IntUserSetting) setting;
                            var actions = new StringPropertyAtom.UiPropertyActions<float>
                            {
                                OnChange = _ => { },
                                OnInvalid = () => { },
                                OnAbort = _ => { },
                                OnSubmit = value => { concreteSetting.Set((int) value); }
                            };
                            categoryPanel.AddNumberProperty(concreteSetting.name, "", concreteSetting.Get(), actions);
                            break;
                        }
                        case (SettingsSystem.SettingStage.Project, SettingsSystem.SettingType.String):
                        {
                            var concreteSetting = (SettingsSystem.StringProjectSetting) setting;
                            var actions = new StringPropertyAtom.UiPropertyActions<string>
                            {
                                OnChange = _ => { },
                                OnAbort = _ => { },
                                OnSubmit = value => { concreteSetting.Set(value); }
                            };
                            categoryPanel.AddStringProperty(concreteSetting.name, "", concreteSetting.Get(), actions);
                            break;
                        }
                        case (SettingsSystem.SettingStage.Project, SettingsSystem.SettingType.Vector3):
                        {
                            var concreteSetting = (SettingsSystem.Vec3ProjectSetting) setting;
                            var actions = new StringPropertyAtom.UiPropertyActions<Vector3>
                            {
                                OnChange = _ => { },
                                OnInvalid = () => { },
                                OnAbort = _ => { },
                                OnSubmit = value => { concreteSetting.Set(value); }
                            };
                            categoryPanel.AddVector3Property(concreteSetting.name, new List<string> {"", "", ""}, concreteSetting.Get(), actions);
                            break;
                        }
                        case (SettingsSystem.SettingStage.Scene, SettingsSystem.SettingType.Vector3):
                        {
                            var concreteSetting = (SettingsSystem.Vec3SceneSetting) setting;
                            var actions = new StringPropertyAtom.UiPropertyActions<Vector3>
                            {
                                OnChange = _ => { },
                                OnInvalid = () => { },
                                OnAbort = _ => { },
                                OnSubmit = value => { concreteSetting.Set(value); }
                            };
                            categoryPanel.AddVector3Property(concreteSetting.name, new List<string> {"", "", ""}, concreteSetting.Get(), actions);
                            break;
                        }
                        default:
                            throw new Exception($"No Setting Type available for stage {setting.stage} and type {setting.type}");
                    }
                }
            }

            settingsPanel.AddSpacer(100);
            settingsPanel.AddText($"dcl-edit version: {Application.version}");

            uiBuilder.Update(settingsPanel);
        }
    }
}