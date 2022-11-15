using System.IO;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class PathState
{
    [CanBeNull]
    private string _projectPath = null;
    public string ProjectPath
    {
        get
        {
            if (_projectPath != null)
                return _projectPath;

#if UNITY_EDITOR

            var devProjectPathFilePath = Application.dataPath + "/dev_project_path.txt";
            if (File.Exists(devProjectPathFilePath))
            {
                _projectPath = File.ReadAllText(devProjectPathFilePath);
            }

            if (!File.Exists(_projectPath + "/scene.json"))
            {
                _projectPath = EditorUtility.OpenFolderPanel("Select DCL project folder", "", "");
                File.WriteAllText(devProjectPathFilePath, _projectPath);
            }

#else
            _projectPath = Path.GetFullPath(".");
#endif
            return _projectPath;
        }
    }

    public string CachePath = null;
}
