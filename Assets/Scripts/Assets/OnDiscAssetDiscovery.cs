using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Assets.Scripts.Assets;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

public class OnDiscAssetDiscovery
{
    // Dependencies
    private DiscoveredAssets discoveredAssets;
    private FileManagerSystem fileManagerSystem;
    private IPathState pathState;
    private AssetMetaFile assetMetaFile;
    private EditorEvents editionEvents;

    [Inject]
    private void Construct(
        DiscoveredAssets discoveredAssets,
        FileManagerSystem fileManagerSystem,
        IPathState pathState,
        AssetMetaFile assetMetaFile,
        EditorEvents editorEvents)
    {
        this.discoveredAssets = discoveredAssets;
        this.fileManagerSystem = fileManagerSystem;
        this.pathState = pathState;
        this.assetMetaFile = assetMetaFile;
        this.editionEvents = editorEvents;
    }

    private Dictionary<string, Guid> pathToIdStore = new();

    public void Initialize()
    {
        fileManagerSystem.ForAllFilesByFilterNowAndOnChange(Path.Combine(pathState.ProjectPath, "assets"), "*", (eventType, filePath, oldFilePath) =>
        {
            // ignore unknown files including meta files
            if (!IsKnownAssetFile(filePath)) return;

            // check if deleted
            if (eventType is FileManagerSystem.FileWatcherEvent.Deleted)
            {
                if (pathToIdStore.TryGetValue(filePath, out var id))
                {
                    if (discoveredAssets.discoveredAssets.ContainsKey(id))
                    {
                        discoveredAssets.discoveredAssets.Remove(id);
                        pathToIdStore.Remove(filePath);
                    }
                }
            }
            else if (eventType is FileManagerSystem.FileWatcherEvent.Renamed)
            {
                //if (pathToIdStore.TryGetValue(oldFilePath, out var id))
                //{
                //    if (discoveredAssets.discoveredAssets.ContainsKey(id))
                //    {
                //        Assert.IsTrue(discoveredAssets.discoveredAssets[id].baseFormat is AssetFormatOnDisc);
                //        var baseFormat = (AssetFormatOnDisc) discoveredAssets.discoveredAssets[id].baseFormat;
                //
                //        
                //            
                //            
                //        pathToIdStore.Remove(oldFilePath);
                //        pathToIdStore.Add(filePath, id);
                //    }
                //}
            }
            else
            {
                // try to find the meta file for the asset
                const string metaFileExtension = ".dclasset";
                var metaFilePath = filePath + metaFileExtension;
                AssetMetaFile.Structure metaFile;
                if (File.Exists(metaFilePath))
                {
                    metaFile = assetMetaFile.ReadFile(metaFilePath);
                }
                else
                {
                    // if no meta file exist, make a new one
                    metaFile = assetMetaFile.MakeStructure(Path.GetFileName(filePath), filePath, AssetTypeStringByExtension(filePath));
                    assetMetaFile.WriteFile(metaFilePath, metaFile);
                }

                // extract values for easier access
                var assetId = Guid.Parse(metaFile.metadata.assetId);

                // try to find asset info
                if (discoveredAssets.discoveredAssets.TryGetValue(assetId, out var existingAssetInfo))
                {
                    Assert.IsTrue(existingAssetInfo.baseFormat is AssetFormatOnDisc);
                    var formatOnDisc = (AssetFormatOnDisc) existingAssetInfo.baseFormat;

                    if (eventType == FileManagerSystem.FileWatcherEvent.Renamed)
                    {
                        formatOnDisc.SetPaths(metaFilePath, filePath);
                    }

                    if (formatOnDisc.UpdateHash())
                    {
                        existingAssetInfo.InvokeAssetFormatChanged();
                    }
                }
                else
                {
                    // make and add asset info
                    // asset path relative to the assets folder
                    var displayPath = Path.GetRelativePath(pathState.AssetPath, filePath);
                    displayPath = Path.GetDirectoryName(displayPath);

                    // create base format
                    var baseFormat = new AssetFormatOnDisc(metaFilePath, filePath);

                    // create asset info
                    var newAssetInfo = new CommonAssetTypes.AssetInfo
                    {
                        assetId = assetId,
                        assetName = metaFile.metadata.assetDisplayName,
                        assetSource = CommonAssetTypes.AssetSource.Local,
                        assetType = AssetTypeByAssetTypeString(metaFile.metadata.assetType),
                        availableFormats = new List<CommonAssetTypes.AssetFormat> {baseFormat},
                        baseFormat = baseFormat,
                        dependencies = new List<CommonAssetTypes.AssetInfo>(),
                        displayPath = displayPath,
                        visible = true
                    };

                    // add asset info
                    discoveredAssets.discoveredAssets.Add(newAssetInfo.assetId, newAssetInfo);
                    pathToIdStore.Add(filePath, newAssetInfo.assetId);
                }
            }
        });
    }

    public bool IsKnownAssetFile(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        extension = extension.TrimStart(' ', '.');
        extension = extension.ToLower();

        return extension switch
        {
            "glb" => true,
            "gltf" => true,
            "png" => true,
            "jpg" => true,
            "jpeg" => true,
            _ => false
        };
    }

    public string AssetTypeStringByExtension(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        extension = extension.TrimStart(' ', '.');
        extension = extension.ToLower();

        return extension switch
        {
            "glb" => "Model",
            "gltf" => "Model",
            "png" => "Image",
            "jpg" => "Image",
            "jpeg" => "Image",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public CommonAssetTypes.AssetType AssetTypeByAssetTypeString(string assetTypeString)
    {
        return assetTypeString switch
        {
            "Model" => CommonAssetTypes.AssetType.Model3D,
            "Image" => CommonAssetTypes.AssetType.Image,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
