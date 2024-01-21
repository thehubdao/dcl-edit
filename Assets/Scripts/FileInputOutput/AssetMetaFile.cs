using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.System;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public class AssetMetaFile
{
    // Dependencies
    private FileUpgraderSystem fileUpgraderSystem;

    [Inject]
    private void Construct(FileUpgraderSystem fileUpgraderSystem)
    {
        this.fileUpgraderSystem = fileUpgraderSystem;
    }

    [Serializable]
    public struct StructureMetaData
    {
        public string @assetFilename;
        public string @assetId;
        public string @assetType;
        public string @assetDisplayName;
    }

    [Serializable]
    public struct Structure
    {
        public StructureMetaData @metadata;
        public string @dclEditVersionNumber;
        public string @thumbnail;
    }

    public Structure ReadFile(string path)
    {
        var text = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<Structure>(text);
    }

    public void WriteFile(string path, Structure structure)
    {
        var text = JsonConvert.SerializeObject(structure, Formatting.Indented);
        File.WriteAllText(path, text);
    }

    public Structure MakeStructure(string displayName, string fileName, string type)
    {
        return new Structure
        {
            dclEditVersionNumber = fileUpgraderSystem.dclAssetVersion.ToString(),
            thumbnail = "",
            metadata = new StructureMetaData
            {
                assetDisplayName = displayName,
                assetFilename = fileName,
                assetId = Guid.NewGuid().ToString(),
                assetType = type
            }
        };
    }
}
