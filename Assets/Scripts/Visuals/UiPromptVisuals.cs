using System;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets.Scripts.EditorState;
using HSVPicker;
using System.Linq;

public class UiPromptVisuals : MonoBehaviour
{
    [SerializeField]
    private GameObject content;

    [SerializeField]
    private RectTransform contentRectTransform;

    [SerializeField]
    private CanvasGroup mainCanvasGroup;

    private UiBuilder uiBuilder;
    private UiAssetBrowserVisuals.Factory uiAssetBrowserViusalsFactory;
    private EditorEvents editorEvents;
    private DialogState dialogState;
    private SceneManagerSystem sceneManagerSystem;
    private CommandSystem commandSystem;
    private DialogSystem dialogSystem;
    private AssetBrowserState assetBrowserState;
    private UnityState unityState;
    private AvailableComponentsState availableComponentsState;

    private PanelAtom.Data panel;
    public Data data;

    private GameObject tmpObject;

    [Inject]
    void Construct(
        UiBuilder.Factory uiBuilderFactory,
        UiAssetBrowserVisuals.Factory uiAssetBrowserViusalsFactory,
        EditorEvents editorEvents,
        DialogState dialogState,
        SceneManagerSystem sceneManagerSystem,
        CommandSystem commandSystem,
        AssetBrowserState assetBrowserState,
        DialogSystem dialogSystem,
        UnityState unityState,
        AvailableComponentsState availableComponentsState)
    {
        uiBuilder = uiBuilderFactory.Create(content);
        this.uiAssetBrowserViusalsFactory = uiAssetBrowserViusalsFactory;
        this.editorEvents = editorEvents;
        this.dialogState = dialogState;
        this.sceneManagerSystem = sceneManagerSystem;
        this.commandSystem = commandSystem;
        this.dialogSystem = dialogSystem;
        this.assetBrowserState = assetBrowserState;
        this.unityState = unityState;
        this.availableComponentsState = availableComponentsState;
    }

    public static void CreateText(PanelAtom.Data panel, Data data)
    {
        if (!string.IsNullOrEmpty(data.dialogText))
            panel.AddText(data.dialogText);
    }

    public static void AssignActions(Data data)
    {
        foreach (var action in data.actions)
            action.data = data;
        if (data.notInWindowAction != null)
            data.notInWindowAction.data = data;
    }

    public static void AddButtons(PanelAtom.Data panel, Data data)
    {
        var horizontalPanel = panel.AddPanel(PanelHandler.LayoutDirection.Horizontal, TextAnchor.UpperCenter);
        foreach (var action in data.actions)
            horizontalPanel.AddButton(action.name, new LeftClickStrategy(e => action.Submit()));
    }

    public void ActivateDialog(string dialogText, PromptSystem.Action[] actions, PromptSystem.Action notInWindowAction)
    {
        data = new(gameObject, dialogText, actions, notInWindowAction);
        panel = UiBuilder.NewPanelData();
        CreateText(panel, data);
        AssignActions(data);
        AddButtons(panel, data);
        uiBuilder.Update(panel);
        gameObject.SetActive(true);
    }

    public ColorPicker ActivateColorPicker(string dialogText, GameObject uiElement)
    {
        if (tmpObject != null)
            Destroy(tmpObject);

        tmpObject = Instantiate(uiElement);
        var colorPicker = tmpObject.GetComponent<ColorPicker>();
        PromptSystem.Value<Color> GetColor = new(null);
        GetColor.action = SetColor;
        data = new(gameObject, dialogText, null, GetColor);
        panel = UiBuilder.NewPanelData();
        CreateText(panel, data);
        AssignActions(data);
        uiBuilder.Update(panel);
        tmpObject.transform.SetParent(content.transform, false);
        gameObject.SetActive(true);
        return colorPicker;

        void SetColor()
        {
            GetColor.value = colorPicker.CurrentColor;
            Destroy(tmpObject);
        }
    }

    public void ActivateAssetBrowser(string dialogText, DclPropertyIdentifier propertyIdentifier)
    {
        // Setup the asset filter
        assetBrowserState.StoreShownTypesTemp();

        var component = sceneManagerSystem.GetCurrentScene().GetEntityById(propertyIdentifier.Entity).GetComponentByName(propertyIdentifier.Component);
        var property = component.GetPropertyByName(propertyIdentifier.Property);

        var componentDefinition = availableComponentsState.GetComponentDefinitionByName(component.NameInCode);
        var propertyDefinition = componentDefinition.properties.First(p => p.name == propertyIdentifier.Property);

        if ((propertyDefinition.flags & DclComponent.DclComponentProperty.PropertyDefinition.Flags.ModelAssets) != 0)
        {
            // Model asset
            assetBrowserState.shownAssetTypes.Add(AssetMetadata.AssetType.Model);
        }

        if ((propertyDefinition.flags & DclComponent.DclComponentProperty.PropertyDefinition.Flags.SceneAssets) != 0)
        {
            // Scene asset
            assetBrowserState.shownAssetTypes.Add(AssetMetadata.AssetType.Scene);
        }

        // Create the asset browser visuals
        if (tmpObject != null)
        {
            Destroy(tmpObject);
            data.window.SetActive(false);
            return;
        }

        panel = UiBuilder.NewPanelData();
        var uiAssetBrowserVisuals = uiAssetBrowserViusalsFactory.Create();
        tmpObject = uiAssetBrowserVisuals.gameObject;
        UiAssetBrowserVisuals visuals = uiAssetBrowserVisuals.GetComponent<UiAssetBrowserVisuals>();

        PromptSystem.Value<Guid> GetGuid = new(() => Destroy(tmpObject));
        data = new(gameObject, dialogText, null, GetGuid);

        AssignActions(data);
        CreateText(panel, data);


        visuals.assetButtonOnClickOverride = (Guid assetId) =>
        {
            if (assetId != Guid.Empty)
            {
                // Update the target component with the new asset
                if (property != null)
                {
                    var oldValue = property.GetConcrete<Guid>().FixedValue;
                    commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangePropertyCommand(propertyIdentifier, oldValue, assetId));
                }
            }

            dialogSystem.CloseCurrentDialog();
            assetBrowserState.RestoreShownTypes();
            editorEvents.InvokeSelectionChangedEvent();
            editorEvents.InvokeUiChangedEvent();
            GetGuid.value = assetId;
            data.notInWindowAction?.Submit();
        };

        //this is not nice
        var layoutElement = uiAssetBrowserVisuals.gameObject.AddComponent<LayoutElement>();
        layoutElement.minHeight = 400;
        layoutElement.minWidth = 600;
        uiBuilder.Update(panel);
        uiAssetBrowserVisuals.transform.SetParent(content.transform, false);
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (RectTransformUtility.RectangleContainsScreenPoint(contentRectTransform, Input.mousePosition)) return;
        data.notInWindowAction?.Submit();
    }

    private void OnEnable()
    {
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

    public class Data
    {
        public GameObject window;
        public string dialogText;
        public PromptSystem.Action[] actions;
        public PromptSystem.Action notInWindowAction;
        public TaskCompletionSource<PromptSystem.Action> taskCompleted = new();

        public Data(GameObject window, string dialogText, PromptSystem.Action[] actions, PromptSystem.Action notInWindowAction)
        {
            this.window = window;
            this.dialogText = dialogText ?? "";
            this.actions = actions ?? new PromptSystem.Action[0];
            this.notInWindowAction = notInWindowAction;
        }
    }
}
