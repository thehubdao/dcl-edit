using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class FileAssetLoaderSystem : IAssetLoaderSystem
    {
        // Dependencies
        private FileAssetLoaderState loaderState;
        private PathState pathState; // TODO: Change this
        private EditorEvents editorEvents;
        private FileUpgraderSystem fileUpgraderSystem;
        private AssetCacheSystem assetCacheSystem;
        private GltfFileFormat.Factory gltfFileFormatFactory;

        private string relativePathInProject = "assets";
        private bool readOnly = false;

        [Inject]
        private void Construct(
            FileAssetLoaderState loaderState,
            PathState pathState,
            EditorEvents editorEvents,
            FileUpgraderSystem fileUpgraderSystem,
            AssetCacheSystem assetCacheSystem,
            GltfFileFormat.Factory gltfFileFormatFactory)
        {
            this.loaderState = loaderState;
            this.pathState = pathState;
            this.editorEvents = editorEvents;
            this.fileUpgraderSystem = fileUpgraderSystem;
            this.assetCacheSystem = assetCacheSystem;
            this.gltfFileFormatFactory = gltfFileFormatFactory;

            editorEvents.onAssetDataUpdatedEvent += OnAssetDataUpdatedCallback;
        }

        /// <summary>
        /// Note: The relative path should never start with a slash (/). Otherwise combining paths with Path.Combine()
        /// won't work since it's parameter "path2" shouldn't be absolute.
        /// </summary>
        /// <param name="relativePathInProject"></param>
        /// <param name="readOnly"></param>
        public FileAssetLoaderSystem(string relativePathInProject, bool readOnly)
        {
            this.relativePathInProject = relativePathInProject;
            this.readOnly = readOnly;
        }

        //TODO Change when asset loading is changed
        private void CheckAssetDirectoryExists()
        {
            string directoryPath = Path.Combine(pathState.ProjectPath, relativePathInProject);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public void ClearAllData()
        {
            foreach (Guid id in loaderState.assetIds)
            {
                assetCacheSystem.Remove(id);
            }
            loaderState.assetIds.Clear();

            loaderState.assetHierarchy = new AssetHierarchyItem();
        }

        public void CacheAllAssetMetadata()
        {
            CheckAssetDirectoryExists();

            ClearAllData();
            try
            {
                string directoryPath = Path.Combine(pathState.ProjectPath, relativePathInProject);

                loaderState.assetHierarchy = ScanDirectory(directoryPath, overrideDirname: StringToTitleCase(relativePathInProject));

                editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
            }
            catch (Exception e)
            {
                Debug.Log($"Error while caching assets: {e}");
            }
        }

        public IEnumerable<Guid> GetAllAssetIds() => loaderState.assetIds;

        public AssetHierarchyItem GetHierarchy()
        {
            if (loaderState.assetHierarchy == null)
            {
                CacheAllAssetMetadata();
            }
            return loaderState.assetHierarchy;
        }

        public Task<string> CopyAssetTo(Guid id)
        {
            MetadataFileFormat mff = assetCacheSystem.GetMetadata(id);
            if (mff != null)
            {
                return Task.FromResult(StaticUtilities.MakeRelativePath(pathState.ProjectPath, mff.assetFilePath));
            }

            return Task.FromResult<string>(null);
        }

        private AssetHierarchyItem ScanDirectory(string fileSystemPath, string pathInHierarchy = "", string overrideDirname = null)
        {
            CheckUpgrades(Directory.GetFiles(fileSystemPath, "*.dclasset"));

            string dirname = Path.GetFileName(fileSystemPath);
            string[] files = Directory.GetFiles(fileSystemPath, "*.*");
            string[] subdirs = Directory.GetDirectories(fileSystemPath);

            pathInHierarchy += "/" + dirname;

            List<AssetMetadata> assets = new List<AssetMetadata>();
            List<AssetHierarchyItem> childDirectories = new List<AssetHierarchyItem>();


            foreach (string assetFile in files)
            {
                AssetMetadata result = ScanFile(assetFile);
                if (result != null) assets.Add(result);
            }

            foreach (string subdir in subdirs)
            {
                // Treat scenes as assets instead of directories
                if (Path.GetExtension(subdir) == ".dclscene")
                {
                    var sceneMetadataFile = ReadSceneDirectory(subdir);
                    if (sceneMetadataFile != null)
                    {
                        assetCacheSystem.Add(sceneMetadataFile.id, sceneMetadataFile);
                        assets.Add(
                            new AssetMetadata(
                                sceneMetadataFile.contents.metadata.assetDisplayName,
                                sceneMetadataFile.id,
                                sceneMetadataFile.contents.metadata.assetType
                                ));
                    }
                    continue;
                }

                childDirectories.Add(ScanDirectory(subdir, pathInHierarchy));
            }

            return new AssetHierarchyItem(overrideDirname ?? dirname, pathInHierarchy, childDirectories, assets);
        }

        private string StringToTitleCase(string dirname) => new CultureInfo("en-US", false).TextInfo.ToTitleCase(dirname);

        private void CheckUpgrades(string[] files)
        {
            foreach (var assetFile in files)
            {
                fileUpgraderSystem.CheckUpgrades(assetFile);
            }
        }

        private MetadataFileFormat ReadSceneDirectory(string pathToScene)
        {
            try
            {
                string sceneName = Path.GetFileNameWithoutExtension(pathToScene);
                string pathToSceneFile = Path.Combine(pathToScene, "scene.json");
                string rawContents = File.ReadAllText(pathToSceneFile);
                SceneManagerSystem.SceneFileContents sceneFileContents = JsonConvert.DeserializeObject<SceneManagerSystem.SceneFileContents>(rawContents);
                MetadataFileFormat mff = new MetadataFileFormat(
                    sceneFileContents.id,
                    sceneFileContents.relativePath,
                    new MetadataFileFormat.Contents(
                        new MetadataFileFormat.AssetInfo(sceneFileContents.id, sceneName, sceneName, AssetMetadata.AssetType.Scene),
                        sceneFileContents.dclEditVersionNumber
                    ));
                return mff;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        protected AssetMetadata ScanFile(string pathToFile)
        {
            string fileName = Path.GetFileName(pathToFile);
            string displayName = Path.GetFileNameWithoutExtension(pathToFile);
            string fileNameExtension = Path.GetExtension(pathToFile);
            AssetMetadata result = null;
            try
            {
                switch (fileNameExtension)
                {
                    case ".dclasset":
                        // New cache system
                        string json = File.ReadAllText(pathToFile);
                        MetadataFileFormat.Contents contents = JsonConvert.DeserializeObject<MetadataFileFormat.Contents>(json);
                        MetadataFileFormat metaFormat = new MetadataFileFormat(contents.metadata.assetId, pathToFile, contents);
                        assetCacheSystem.Add(contents.metadata.assetId, metaFormat);
                        // Change this line so that only the new cache system is used
                        result = new AssetMetadata(contents.metadata.assetDisplayName, contents.metadata.assetId, contents.metadata.assetType);
                        break;
                    case ".glb":
                    case ".gltf":
                        MetadataFileFormat metaFile = null;

                        if (MetadataFileExists(pathToFile))
                        {
                            string json1 = File.ReadAllText(pathToFile + ".dclasset");
                            MetadataFileFormat.Contents con = JsonConvert.DeserializeObject<MetadataFileFormat.Contents>(json1);
                            metaFile = new MetadataFileFormat(con.metadata.assetId, pathToFile + ".dclasset", con);
                        }
                        else if (!MetadataFileExists(pathToFile) && !readOnly)
                        {
                            // Create a new metadata file, add it to cache and write it to filesystem
                            Guid newId = Guid.NewGuid();
                            metaFile = new MetadataFileFormat(
                                newId,
                                pathToFile + ".dclasset",
                                new MetadataFileFormat.Contents
                                (
                                    new MetadataFileFormat.AssetInfo(
                                        newId,
                                        fileName,
                                        displayName,
                                        AssetMetadata.AssetType.Model
                                    ),
                                    Application.version
                                ));
                        }

                        // Add asset file to cache
                        assetCacheSystem.Add(metaFile.id, metaFile);
                        assetCacheSystem.Add(metaFile.id, gltfFileFormatFactory.Create(metaFile.id, pathToFile));
                        break;
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return null;
        }

        /// <summary>
        /// Checks if there is a metadata file (.dclasset) that belongs to the given asset file.
        /// </summary>
        /// <param name="pathToAssetFile"></param>
        /// <returns></returns>
        private bool MetadataFileExists(string pathToAssetFile) => File.Exists(pathToAssetFile + ".dclasset");

        /// <summary>
        /// Writes the given metadata to a .dclasset or scene.json file.
        /// </summary>
        /// <param name="metadataFormat"></param>
        private void WriteMetadataToFile(MetadataFileFormat metadataFormat)
        {
            try
            {
                if (metadataFormat.contents.metadata.assetType != AssetMetadata.AssetType.Scene)
                {
                    string json = JsonConvert.SerializeObject(metadataFormat.contents, Formatting.Indented);
                    File.WriteAllText(metadataFormat.pathToMetadataFile, json);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while writing metadata to file: {e}");
            }
        }

        /// <summary>
        /// Checks if the specified asset is an asset on the file system. If it is, then the 
        /// updated metadata is written to the corresponding .dclasset file.
        /// </summary>
        /// <param name="id"></param>
        public async void OnAssetDataUpdatedCallback(Guid id)
        {
            MetadataFileFormat mff = assetCacheSystem.GetMetadata(id);
            if (mff == null) return;

            Texture2D thumbnail = await assetCacheSystem.GetThumbnail(id);

            if (thumbnail != null)
            {
                // Update the thumbnail string in the metadata.
                mff.contents = new MetadataFileFormat.Contents(mff.contents.metadata, mff.contents.dclEditVersionNumber, thumbnail);
            }

            WriteMetadataToFile(mff);
        }
    }
}
