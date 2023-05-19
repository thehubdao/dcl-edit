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

public class UiPromptVisuals : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject content;
    [SerializeField] private RectTransform contentRectTransform;
    
    private UiBuilder uiBuilder;
    private UiAssetBrowserVisuals.Factory uiAssetBrowserViusalsFactory;
    private EditorEvents editorEvents;
    private DialogState dialogState;
    private SceneManagerSystem sceneManagerSystem;
    private CommandSystem commandSystem;
    private DialogSystem dialogSystem;
    private AssetBrowserState assetBrowserState;

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
    DialogSystem dialogSystem)
    {
        uiBuilder = uiBuilderFactory.Create(content);
        this.uiAssetBrowserViusalsFactory = uiAssetBrowserViusalsFactory;
        this.editorEvents = editorEvents;
        this.dialogState = dialogState;
        this.sceneManagerSystem = sceneManagerSystem;
        this.commandSystem = commandSystem;
        this.dialogSystem = dialogSystem;
        this.assetBrowserState = assetBrowserState;
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
        data.notInWindowAction.data = data;
    }

    public static void AddButtons(PanelAtom.Data panel, Data data)
    {
        var horizontalPanel = panel.AddPanel(PanelHandler.LayoutDirection.Horizontal, TextAnchor.UpperCenter);
        foreach (var action in data.actions)
            horizontalPanel.AddButton(action.name, action.Submit);
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
        tmpObject.transform.SetParent(content.transform);
        gameObject.SetActive(true);
        return colorPicker;

        void SetColor()
        {
            GetColor.value = colorPicker.CurrentColor;
            Destroy(tmpObject);
        }
    }

    public void ActivateAssetBrowser(string dialogText)
    {
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
        uiBuilder.Update(panel);
        uiAssetBrowserVisuals.transform.SetParent(content.transform);
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (RectTransformUtility.RectangleContainsScreenPoint(contentRectTransform, Input.mousePosition)) return;
        data.notInWindowAction?.Submit(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        dialogState.mouseOverDialogWindow = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
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
