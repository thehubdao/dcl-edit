using System;
using UnityEngine;

public static class TMPDclEditHelpers
{
    //Insert all TMP <link> handling options in this class.
    //The specific behavior should be chosen outside of this class.
    
    public static void OpenUrl(string obtainedLinkId)
    {
        if (!Enum.TryParse(obtainedLinkId, out UrlId urlId))
        {
            Debug.Log("Wrong linkId: " + "obtained LinkId: " + obtainedLinkId);
            return;
        }

        Application.OpenURL(Config.links[urlId]);
    }
}
