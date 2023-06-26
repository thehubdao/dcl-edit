using Assets.Scripts.SceneState;

public class DialogState
{
    public enum DialogType
    {
        None,
        Asset,
        DialogSystem
    }
    public DialogType currentDialog = DialogType.None;
    public DclComponent targetComponent;
    public bool mouseOverDialogWindow;
}
