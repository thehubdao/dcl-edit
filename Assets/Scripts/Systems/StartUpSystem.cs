using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class StartUpSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject _noProjectWindow;

    

    private static void SetProjectPath()
    {
#if UNITY_EDITOR
        
        var devProjectPathFilePath = Application.dataPath+"/dev_project_path.txt";
        if (File.Exists(devProjectPathFilePath))
        {
            DclSceneManager.DclProjectPath = File.ReadAllText(devProjectPathFilePath);
        }

        if (!File.Exists(DclSceneManager.DclProjectPath + "/scene.json"))
        {
            DclSceneManager.DclProjectPath = EditorUtility.OpenFolderPanel("Select DCL project folder","","");
            File.WriteAllText(devProjectPathFilePath,DclSceneManager.DclProjectPath);
        }

#else
        DclSceneManager.DclProjectPath = Path.GetFullPath(".");
#endif
    }

    void Start()
    {
        SetProjectPath();

        if (!File.Exists(DclSceneManager.DclProjectPath + "/scene.json"))
        {
            Debug.LogError("No Project found in selected Path");
            _noProjectWindow.SetActive(true);
            return;
        }

        var sr = new StreamReader(DclSceneManager.DclProjectPath + "/scene.json");
        var fileContents = sr.ReadToEnd();
        sr.Close();

        DclSceneManager.sceneJson = JsonUtility.FromJson<DclSceneManager.SceneJson>(fileContents);

        var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var rootObject in rootObjects)
        {
            foreach (var staticSerializer in rootObject.GetComponentsInChildren<ISerializedFieldToStatic>())
            {
                staticSerializer.SetupStatics();
            }
        }

        CameraManager.ChooseReasonableStartPosition();

        SaveFileUpgrader.CheckSaveFiles();

        AssetSaverSystem.Load();
        
        DclSceneManager.ChangedHierarchy();

        DclSceneManager.SetSelection(null);
        
        SceneSaveSystem.Load();

        //AssetBrowserManager.OpenAssetBrowser(
        //    (asset) => { Debug.Log(asset); },
        //    () => { Debug.LogError("Error while selecting asset"); }
        //    );
        //AssetBrowserManager.OpenAssetBrowser();

    }
}
