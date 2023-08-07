using TMPro;
using UnityEngine;

public static class TMPDclEditExtensions 
{
    private const int invalidLinkIndex = -1;

    public static bool TryGetLinkID(this TMP_Text textComponent, out string linkID)
    {
        linkID = null;

        if (!TryGetRect(textComponent, out var inputPosition))
        {
            return false;
        }

        var linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, inputPosition, null);

        if (linkIndex == invalidLinkIndex)
        {
            return false;
        }
        
        var linkInfos = textComponent.textInfo.linkInfo;

        if (linkIndex >= linkInfos.Length)
        {
            return false;
        }
 
        linkID = linkInfos[linkIndex].GetLinkID();
            
        return true;
    }


    private static bool TryGetRect(this TMP_Text textComponent, out Vector2 inputPosition)
    {
        if (Input.GetMouseButtonUp(0))
        {
            inputPosition = Input.mousePosition;
            return TMP_TextUtilities.IsIntersectingRectTransform(textComponent.rectTransform, inputPosition, null);
        }

        inputPosition = default;
        return false;
    }
}
