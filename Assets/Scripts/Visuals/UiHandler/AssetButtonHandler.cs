using UnityEngine.UI;

public class AssetButtonHandler : ButtonHandler
{
    public Image maskedImage;       // Uses a child object with an image component. This allows setting an image that is influenced by the buttons mask.
    public Image assetTypeIndicator;
    public AssetButtonInteraction assetButtonInteraction;
}