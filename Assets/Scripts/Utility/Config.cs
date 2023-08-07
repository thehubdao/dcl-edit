using System.Collections.Generic;

public static class Config
{
    public static readonly Dictionary<UrlId, string> links
    = new Dictionary<UrlId, string>
    {
        { UrlId.InstallationInstructions, "https://dcl-edit.com/install-guide/" },
        { UrlId.CustomComponents, "https://dcl-edit.com/custom-component-markup" }
    };

}

public enum UrlId {
    InstallationInstructions,
    CustomComponents
}