using System;

public class DialogState
{
    public enum DialogType
    {
        None,
        Asset
    }
    public DialogType currentDialog = DialogType.None;
    public Guid targetEntityId;
}
