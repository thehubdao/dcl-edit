using Assets.Scripts.SceneState;

public class DialogState
{
    public enum DialogType
    {
        None,
        Asset,
        DialogSystem
    }
    public Subscribable<DialogType> currentDialog = new(DialogType.None);
    public DclComponent targetComponent;
    public bool mouseOverDialogWindow;
}
