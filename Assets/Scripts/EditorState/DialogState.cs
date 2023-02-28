using Assets.Scripts.SceneState;

public class DialogState
{
    public enum DialogType
    {
        None,
        Asset
    }
    public DialogType currentDialog = DialogType.None;
    public DclComponent targetComponent;
    public bool mouseOverDialogWindow;
}
