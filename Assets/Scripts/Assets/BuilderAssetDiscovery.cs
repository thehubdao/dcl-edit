using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Assets.Scripts.Assets
{
    public class BuilderAssetDiscovery
    {
#pragma warning disable CS0649
// ReSharper disable CollectionNeverUpdated.Local InconsistentNaming
        private struct AssetPacks
        {
            public bool ok;

            public List<AssetPacksData> data;
        }

        private struct AssetPacksData
        {
            public string id;
            public string title;
            public string thumbnail;
            public string created_at;
            public string updated_at;
            public string eth_address;
            public List<AssetPacksAsset> assets;
        }

        private struct AssetPacksAsset
        {
            public string id;
            public string asset_pack_id;
            public string name;
            public string model;
            public string thumbnail;
            public List<string> tags;
            public string category;
            public JObject contents;
            public string created_at;
            public string updated_at;
            public AssetPacksMetrics metrics;
            public string script;
            public JArray parameters;
            public JArray actions;
            public string legacy_id;
        }

        private struct AssetPacksMetrics
        {
            public int triangles;
            public int materials;
            public int textures;
            public int meshes;
            public int bodies;
            public int entities;
        }
// ReSharper restore CollectionNeverUpdated.Local InconsistentNaming
#pragma warning restore CS0649


        // Dependencies
        private DiscoveredAssets discoveredAssets;
        private WebRequestSystem webRequestSystem;
        private EditorEvents editorEvents;

        [Inject]
        private void Construct(
            DiscoveredAssets discoveredAssets,
            WebRequestSystem webRequestSystem,
            EditorEvents editorEvents)
        {
            this.discoveredAssets = discoveredAssets;
            this.webRequestSystem = webRequestSystem;
            this.editorEvents = editorEvents;
        }

        private bool initRun;

        public void Initialize()
        {
            Assert.IsFalse(initRun);

            initRun = true;

            // load all asset metadata from the official builder

            webRequestSystem.Get("https://builder-api.decentraland.org/v1/assetPacks?owner=default", request =>
            {
                var assetData = JsonConvert.DeserializeObject<AssetPacks>(request.webRequest.downloadHandler.text);

                foreach (AssetPacksData assetPack in assetData.data)
                {
                    //string assetPackPath = loaderState.assetHierarchy.path + "/" + assetPack.title;
                    //AssetHierarchyItem assetPackHierarchyItem = new AssetHierarchyItem(assetPack.title, assetPackPath);
                    //
                    //// Builder assets are in categories which are identified by a string name. E.g. "category":"decorations"
                    //Dictionary<String, AssetHierarchyItem> categories = new Dictionary<string, AssetHierarchyItem>();

                    foreach (AssetPacksAsset asset in assetPack.assets)
                    {
                        Guid id = Guid.Parse(asset.id);
                        //
                        //if (!categories.ContainsKey(asset.category))
                        //{
                        //    // Capital first letter
                        //    string name = char.ToUpper(asset.category[0]) + asset.category.Substring(1);
                        //    string path = assetPackPath + "/" + name;
                        //    AssetHierarchyItem categoryHierarchyItem = new AssetHierarchyItem(name, path);
                        //    categories.Add(asset.category, categoryHierarchyItem);
                        //}
                        //
                        //categories[asset.category].assets.Add(new AssetMetadata(asset.name, id, AssetMetadata.AssetType.Model));
                        //
                        //loaderState.Data.Add(id, new BuilderAssetLoaderState.DataStorage
                        //{
                        //    Id = id,
                        //    Name = asset.name,
                        //    //modelHash = asset.contents[asset.Model]?.Value<string>(),
                        //    modelPath = asset.model,
                        //    contentsPathToHash = asset.contents.ToObject<Dictionary<string, string>>(),
                        //    ThumbnailHash = asset.thumbnail
                        //});

                        var contents = asset.contents.ToObject<Dictionary<string, string>>();
                        var modelCloudPath = asset.model;
                        var thumbnailCloudHash = asset.thumbnail;

                        var baseFormat = new AssetFormatBuilderCloud(contents, modelCloudPath, thumbnailCloudHash);

                        discoveredAssets.discoveredAssets.Add(id, new CommonAssetTypes.AssetInfo
                        {
                            assetId = id,
                            assetName = asset.name,
                            assetSource = CommonAssetTypes.AssetSource.DecentralandBuilder,
                            assetType = CommonAssetTypes.AssetType.Model3D,
                            availableFormats = new List<CommonAssetTypes.AssetFormat> {baseFormat},
                            baseFormat = baseFormat,
                            displayPath = $"{assetPack.title}/{asset.category}"
                        });
                    }

                    //foreach (var category in categories)
                    //{
                    //    assetPackHierarchyItem.childDirectories.Add(category.Value);
                    //}

                    //loaderState.assetHierarchy.childDirectories.Add(assetPackHierarchyItem);
                }

                editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
            });
        }
    }
}