using System;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using Assets.Scripts.EditorState;
using HSVPicker;
using Visuals;

namespace Assets.Scripts.Visuals
{
    public class UiPromptVisuals : MonoBehaviour
    {
        [SerializeField] private GameObject content;
        [SerializeField] private RectTransform contentRectTransform;
        [SerializeField] private CanvasGroup mainCanvasGroup;
        
        private UiBuilder.UiBuilder uiBuilder;
        private UiAssetBrowserVisuals.Factory uiAssetBrowserVisualsFactory;
        private EditorEvents editorEvents;
        private DialogState dialogState;
        private SceneManagerSystem sceneManagerSystem;
        private CommandSystem commandSystem;
        private DialogSystem dialogSystem;
        private AssetBrowserState assetBrowserState;
        private UnityState unityState;
        private PromptSystem promptSystem;
        private PanelAtom.Data panel;
        private PromptSystem.PromptData data;
        private GameObject tmpObject;
        private ChangeLogVisuals changeLogVisuals;


        [Inject]
        void Construct(
        UiBuilder.UiBuilder.Factory uiBuilderFactory,
        UiAssetBrowserVisuals.Factory uiAssetBrowserVisualsFactory,
        EditorEvents editorEvents,
        DialogState dialogState,
        SceneManagerSystem sceneManagerSystem,
        CommandSystem commandSystem,
        AssetBrowserState assetBrowserState,
        DialogSystem dialogSystem,
        UnityState unityState,
        PromptSystem promptSystem)
        {
            uiBuilder = uiBuilderFactory.Create(content);
            this.uiAssetBrowserVisualsFactory = uiAssetBrowserVisualsFactory;
            this.editorEvents = editorEvents;
            this.dialogState = dialogState;
            this.sceneManagerSystem = sceneManagerSystem;
            this.commandSystem = commandSystem;
            this.dialogSystem = dialogSystem;
            this.assetBrowserState = assetBrowserState;
            this.unityState = unityState;
            this.promptSystem = promptSystem;
            changeLogVisuals = GetComponent<ChangeLogVisuals>();
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (RectTransformUtility.RectangleContainsScreenPoint(contentRectTransform, Input.mousePosition)) return;
            data.notInWindowAction?.Submit();
        }

        private void OnEnable()
        {
            ActivatePrompt();
            mainCanvasGroup.blocksRaycasts = false;
            mainCanvasGroup.alpha = 0.25f;
            unityState.BackgroundCanvas.SetActive(false);
            dialogState.currentDialog = DialogState.DialogType.DialogSystem;
            dialogState.mouseOverDialogWindow = true;
        }

        private void OnDisable()
        {
            mainCanvasGroup.blocksRaycasts = true;
            mainCanvasGroup.alpha = 1f;
            unityState.BackgroundCanvas.SetActive(true);
            dialogState.currentDialog = DialogState.DialogType.None;
            dialogState.mouseOverDialogWindow = false;
        }

        public void ActivatePrompt()
        {
            if (tmpObject != null)
                Destroy(tmpObject);

            data = promptSystem.promptData;
            panel = UiBuilder.UiBuilder.NewPanelData();
            if (!string.IsNullOrEmpty(data.dialogText))
                panel.AddText(data.dialogText);

            switch(data.promptType)
            {
                case PromptSystem.PromptData.PromptType.Dialog:
                    AddButtons(panel, data);
                    break;
                case PromptSystem.PromptData.PromptType.ColorPicker:
                    AddColorPicker();
                    break;
                case PromptSystem.PromptData.PromptType.AssetPicker:
                    AddAssetBrowser();
                    break;
                case PromptSystem.PromptData.PromptType.ChangeLog:
                    AddChangeLog();
                    break;
            }

            uiBuilder.Update(panel);
        }

        private void AddButtons(PanelAtom.Data panel, PromptSystem.PromptData data)
        {
            var horizontalPanel = panel.AddPanel(PanelHandler.LayoutDirection.Horizontal, TextAnchor.UpperCenter);
            foreach (var action in data.actions)
                horizontalPanel.AddButton(action.name, (gameObject) => action.Submit());
        }

        public void AddColorPicker()
        {
            tmpObject = Instantiate(unityState.ColorPicker);
            var colorPicker = tmpObject.GetComponent<ColorPicker>();
            PromptSystem.Value<Color> ReturnColor = new(null);
            ReturnColor.data = data;
            ReturnColor.action = GetColor;
            data.notInWindowAction = ReturnColor;

            tmpObject.transform.SetParent(content.transform, false);
            Debug.Log(gameObject);
            void GetColor()
            {
                ReturnColor.value = colorPicker.CurrentColor;
                Destroy(tmpObject);
            }
        }

        public void AddAssetBrowser()
        {
            var uiAssetBrowserVisuals = uiAssetBrowserVisualsFactory.Create();
            tmpObject = uiAssetBrowserVisuals.gameObject;
            UiAssetBrowserVisuals visuals = uiAssetBrowserVisuals.GetComponent<UiAssetBrowserVisuals>();

            PromptSystem.Value<Guid> GetGuid = new(() => Destroy(tmpObject));
            GetGuid.data = data;
            data.notInWindowAction = GetGuid;
            assetBrowserState.StoreShownTypesTemp();

            visuals.assetButtonOnClickOverride = (Guid assetId) =>
            {
                DclScene scene = sceneManagerSystem.GetCurrentScene();

                // Update the target component with the new asset
                var currentSelected = scene.SelectionState.PrimarySelectedEntity;
                var targetComponent = currentSelected.GetComponentByName("GLTFShape");
                var sceneProperty = targetComponent.GetPropertyByName("scene");
                var assetProperty = targetComponent.GetPropertyByName("asset");

                if (sceneProperty != null)
                {
                    var oldValue = sceneProperty.GetConcrete<Guid>().FixedValue;
                    var identifier = new DclPropertyIdentifier(targetComponent.Entity.Id, targetComponent.NameInCode, "scene");
                    commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangePropertyCommand(identifier, oldValue, assetId));
                }
                if (assetProperty != null)
                {
                    var oldValue = assetProperty.GetConcrete<Guid>().FixedValue;
                    var identifier = new DclPropertyIdentifier(targetComponent.Entity.Id, targetComponent.NameInCode, "asset");
                    commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangePropertyCommand(identifier, oldValue, assetId));
                }

                dialogSystem.CloseCurrentDialog();
                editorEvents.InvokeSelectionChangedEvent();
                editorEvents.InvokeUiChangedEvent();
                GetGuid.value = assetId;
                data.notInWindowAction?.Submit(null);
            };

            //this is not nice
            var layoutElement = uiAssetBrowserVisuals.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 400;
            layoutElement.minWidth = 600;
            uiAssetBrowserVisuals.transform.SetParent(content.transform, false);
        }
        
        private void AddChangeLog()
        {
            //TODO?
            PromptSystem.Value<bool> destroyValue = new(() => Destroy(tmpObject));
            destroyValue.data = data;
            data.notInWindowAction = destroyValue;

            changeLogVisuals.AddChangeLog();
        }
    }
}