using System;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Zenject;


namespace Assets.Scripts.EditorState
{
    public class PathState : IPathState
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

#else
                //_projectPath = Path.GetFullPath(".");
#endif
                return _projectPath;
            }

            set { _projectPath = value; }
        }

        public string buildPath
        {
            get
            {
                var path = Path.Combine(ProjectPath, "dcl-edit", "build", "assets");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        [Inject]
        private void Construct()
        {
            InterpretArgs(Environment.GetCommandLineArgs());
        }

        private void InterpretArgs(string[] args)
        {
            string projectPath = null;
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "--projectPath")
                {
                    projectPath = args[i + 1];
                }
            }

            if (projectPath != null)
            {
                ProjectPath = projectPath;
            }
        }
    }
}
