using System.IO;
using Assets.Scripts.EditorState;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Tests.EditModeTests.TestUtility
{
    public class MockPathState : IPathState
    {
        public MockPathState(string testProject, bool makeTemporaryProject = false)
        {
            ProjectPath = Application.dataPath + "/../../dcl-edit-testing/projects/" + testProject;

            if (makeTemporaryProject)
            {
                MakeTemporaryCopyOfProject();
            }
        }

        public string ProjectPath { get; set; }

        /**
         * Changes ProjectPath to a new temporary folder and copies the content from the old ProjectPath to the new folder.
         * Use this, when a test requires a change in the project, like saving a scene
         */
        public void MakeTemporaryCopyOfProject()
        {
            var newProjectPath = Application.temporaryCachePath + "/tmp_project_" + Random.Range(int.MinValue, int.MaxValue);

            Directory.CreateDirectory(newProjectPath);
            CopyFilesRecursively(ProjectPath, newProjectPath);

            ProjectPath = newProjectPath;
        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}
