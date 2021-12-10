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
            SceneManager.DclProjectPath = File.ReadAllText(devProjectPathFilePath);
        }

        if (!File.Exists(SceneManager.DclProjectPath + "/scene.json"))
        {
            SceneManager.DclProjectPath = EditorUtility.OpenFolderPanel("Select DCL project folder","","");
            File.WriteAllText(devProjectPathFilePath,SceneManager.DclProjectPath);
        }
        
#else
        SceneManager.DclProjectPath = Path.GetFullPath(".");
#endif
    }

    void Start()
    {
        SetProjectPath();

        if (!File.Exists(SceneManager.DclProjectPath + "/scene.json"))
        {
            Debug.LogError("No Project found in selected Path");
            _noProjectWindow.SetActive(true);
            return;
        }

        var sr = new StreamReader(SceneManager.DclProjectPath + "/scene.json");
        var fileContents = sr.ReadToEnd();
        sr.Close();

        SceneManager.sceneJson = JsonUtility.FromJson<SceneManager.SceneJson>(fileContents);

        var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var rootObject in rootObjects)
        {
            foreach (var staticSerializer in rootObject.GetComponentsInChildren<ISerializedFieldToStatic>())
            {
                staticSerializer.SetupStatics();
            }
        }

        SaveFileUpgrader.CheckSaveFiles();

        AssetSaverSystem.Load();
        
        SceneManager.ChangedHierarchy();

        SceneManager.SetSelection(null);
        
        SceneSaveSystem.Load();

        //AssetBrowserManager.OpenAssetBrowser(
        //    (asset) => { Debug.Log(asset); },
        //    () => { Debug.LogError("Error while selecting asset"); }
        //    );
        //AssetBrowserManager.OpenAssetBrowser();

    }
}
