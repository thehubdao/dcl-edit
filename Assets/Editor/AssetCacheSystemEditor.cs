using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Zenject;

public class AssetCacheSystemEditor : EditorWindow
{
    DclEditorInstaller dclEditorInstaller;
    bool HasDependencies => assetCacheSystem != null;

    Vector2 scrollPos;
    Dictionary<Guid, bool> foldoutState = new Dictionary<Guid, bool>();

    // Dependencies
    AssetCacheSystem assetCacheSystem;

    [Inject]
    public void Construct(AssetCacheSystem assetCacheSystem)
    {
        this.assetCacheSystem = assetCacheSystem;
    }

    [UnityEditor.MenuItem("Window/dcl-edit Cache Debugger")]
    public static void ShowWindow()
    {
        AssetCacheSystemEditor wnd = GetWindow<AssetCacheSystemEditor>();
        wnd.titleContent = new GUIContent("dcl-edit Cache Debugger");
    }

    private void Update()
    {
        if (UnityEngine.Device.Application.isPlaying && !HasDependencies)
        {
            GetDependencies();
        }
        else if (!UnityEngine.Device.Application.isPlaying && HasDependencies)
        {
            ClearDependencies();
        }
    }

    public void OnGUI()
    {

        if (!HasDependencies)
        {
            EditorGUILayout.HelpBox("Please enter Play Mode.", MessageType.Info);
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.HelpBox("This tool displays all cache entries that currently exist in the asset cache.", MessageType.Info);

        foreach (KeyValuePair<Guid, AssetCacheEntry> pair in assetCacheSystem.Entries)
        {
            DrawAssetFormat(pair.Key, pair.Value);
        }
        EditorGUILayout.EndScrollView();
    }

    void GetDependencies()
    {
        dclEditorInstaller = GameObject.Find("SceneContext").GetComponent<DclEditorInstaller>();
        dclEditorInstaller.InjectEditorWindow(this);
    }

    void ClearDependencies()
    {
        assetCacheSystem = null;
    }

    void DrawAssetFormat(Guid id, AssetCacheEntry entry)
    {
        bool foldout = foldoutState.TryGetValue(id, out bool value) ? value : false;
        foldoutState[id] = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, GetAssetName(entry));

        if (foldout)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField($"ID: {id}");
            EditorGUILayout.LabelField($"Formats:");

            EditorGUI.indentLevel++;

            foreach (AssetFormat format in entry.formats)
            {
                switch (format)
                {
                    case MetadataFileFormat mff:
                        DrawMetadataFileFormat(mff);
                        break;
                    case BuilderAssetFormat baf:
                        DrawBuilderAssetFormat(baf);
                        break;
                    case GltfFileFormat gff:
                        DrawGltfFileFormat(gff);
                        break;
                    case LoadedModelFormat lmf:
                        DrawLoadedModelFormat(lmf);
                        break;
                    case ThumbnailFormat tf:
                        DrawThumbnailFormat(tf);
                        break;
                    case SpriteFormat sf:
                        DrawSpriteFormat(sf);
                        break;
                    default:
                        EditorGUILayout.LabelField("Unknown Format: " + format.GetType().ToString());
                        break;
                }
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Convert to:");

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(EditorGUI.indentLevel * 18);

            if (GUILayout.Button("LoadedModelFormat"))
            {
                try
                {
                    // Use ContinueWith instead of await, because awaiting causes an error
                    // when called from inside the BeginHorizontal/EndHorizontal layout.
                    assetCacheSystem.GetLoadedModel(id).ContinueWith(
                        (Task<GameObject> task) =>
                        {
                            if (task.IsCompleted)
                            {
                                GameObject go = task.Result;

                                // The model must be destroyed, because GetLoadedModel() creates a copy,
                                // which is not needed here.
                                if (go != null)
                                {
                                    Destroy(go.gameObject);
                                }
                            }
                        });
                }
                catch { }
            }
            if (GUILayout.Button("ThumbnailFormat"))
            {
                _ = assetCacheSystem.GetThumbnail(id);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }


        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    string GetAssetName(AssetCacheEntry cacheEntry)
    {
        foreach (AssetFormat format in cacheEntry.formats)
        {
            if (format is MetadataFileFormat mff)
            {
                return mff.contents.metadata.assetDisplayName;
            }
            if (format is BuilderAssetFormat baf)
            {
                return baf.Name;
            }
        }
        return "Unkown Name";
    }

    void DrawMetadataFileFormat(MetadataFileFormat mff)
    {
        EditorGUILayout.LabelField(mff.GetType().ToString());

        EditorGUI.indentLevel++;

        EditorGUILayout.LabelField($"Display Name: {mff.contents.metadata.assetDisplayName}");

        EditorGUILayout.LabelField($"Asset File Name: {mff.contents.metadata.assetFilename}");

        EditorGUILayout.LabelField($"Asset Type: {mff.contents.metadata.assetType}");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Metadata File Path (.dclasset file):");
        if (EditorGUILayout.LinkButton(mff.pathToMetadataFile))
        {
            EditorUtility.RevealInFinder(mff.pathToMetadataFile);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField($"Version: {mff.contents.dclEditVersionNumber}");

        EditorGUI.indentLevel--;
    }

    void DrawBuilderAssetFormat(BuilderAssetFormat baf)
    {
        EditorGUILayout.LabelField(baf.GetType().ToString());

        EditorGUI.indentLevel++;

        EditorGUILayout.LabelField($"Name: {baf.Name}");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Model Path:");
        if (EditorGUILayout.LinkButton(baf.modelPath))
        {
            EditorUtility.RevealInFinder(baf.modelPath);
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.LabelField($"Thumbnail Hash: {baf.ThumbnailHash}");

        EditorGUI.indentLevel--;
    }

    void DrawGltfFileFormat(GltfFileFormat gff)
    {
        EditorGUILayout.LabelField(gff.GetType().ToString());

        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("File Path:");
        if (EditorGUILayout.LinkButton(gff.pathToFile))
        {
            EditorUtility.RevealInFinder(gff.pathToFile);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
    }

    void DrawLoadedModelFormat(LoadedModelFormat lmf)
    {
        EditorGUILayout.LabelField(lmf.GetType().ToString());

        EditorGUI.indentLevel++;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Model", lmf.model, typeof(GameObject), allowSceneObjects: true);
        EditorGUI.EndDisabledGroup();

        EditorGUI.indentLevel--;
    }

    void DrawThumbnailFormat(ThumbnailFormat tf)
    {
        EditorGUILayout.LabelField(tf.GetType().ToString());

        EditorGUI.indentLevel++;

        EditorGUILayout.PrefixLabel("Texture:");
        EditorGUI.DrawPreviewTexture(new Rect(GUILayoutUtility.GetLastRect().position + new Vector2(150, 0), new Vector2(50, 50)), tf.thumbnail);
        EditorGUILayout.Space(28);

        EditorGUI.indentLevel--;
    }

    void DrawSpriteFormat(SpriteFormat sf)
    {
        EditorGUILayout.LabelField(sf.GetType().ToString());

        EditorGUI.indentLevel++;

        EditorGUILayout.PrefixLabel("Sprite:");
        EditorGUI.DrawPreviewTexture(new Rect(GUILayoutUtility.GetLastRect().position + new Vector2(150, 0), new Vector2(50, 50)), sf.sprite.texture);
        EditorGUILayout.Space(28);

        EditorGUI.indentLevel--;
    }
}
