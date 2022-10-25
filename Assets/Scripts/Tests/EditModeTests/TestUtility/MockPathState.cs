using Assets.Scripts.EditorState;
using UnityEngine;

namespace Assets.Scripts.Tests.EditModeTests.TestUtility
{
    public class MockPathState : IPathState
    {
        public MockPathState(string testProject)
        {
            ProjectPath = Application.dataPath + "/../../dcl-edit-testing/projects/" + testProject;
        }

        public string ProjectPath { get; }
    }
}
