using System.Collections;
using Assets.Scripts.SceneState;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Assets.Scripts.Tests.PlayModeTests.CommandTests
{
    public class AddEntityTest
    {
        // Dependencies
        private EditorState.SceneFile _sceneFile;

        [OneTimeSetUp]
        private void OneTimeSetUp()
        {
            _sceneFile = new EditorState.SceneFile();
        }


        private IEnumerator SetupScene()
        {
            SceneManager.LoadScene(0);
            yield return null;
            // create new dcl scene
            _sceneFile.CurrentScene = new DclScene();
        }


        [UnityTest]
        public IEnumerator AddingSingleEntity()
        {
            yield return SetupScene();

            var currentScene = _sceneFile.CurrentScene;

            //CommandSystem.ExecuteCommand(new AddEntity("cool name", null));
            //Assert.AreEqual(1, currentScene.AllEntities.Count);
            //
            //var entity = currentScene.AllEntities.First(e => e.Value.ShownName == "cool name");
            //Assert.AreEqual("cool name", entity.Value.ShownName);
            //
            //Assert.AreEqual(currentScene, entity.Value.CurrentScene);
        }

        [UnityTest]
        public IEnumerator AddingEntityWithParent()
        {
            yield return SetupScene();

            var currentScene = _sceneFile.CurrentScene;

            //CommandSystem.ExecuteCommand(new AddEntity("parent", null));
            //var parent = currentScene.AllEntities.First(e => e.Value.ShownName == "parent");
            //CommandSystem.ExecuteCommand(new AddEntity("child", parent.Value));
            //var child = currentScene.AllEntities.First(e => e.Value.ShownName == "child");
            //
            //Assert.AreEqual(2, currentScene.AllEntities.Count);
            //Assert.AreEqual(parent.Value, child.Value.Parent);
            //
            //Assert.AreEqual(currentScene, parent.Value.CurrentScene);
            //Assert.AreEqual(currentScene, child.Value.CurrentScene);
        }

        [UnityTest]
        public IEnumerator AddEntityHistory()
        {
            yield return SetupScene();

            var currentScene = _sceneFile.CurrentScene;

            //CommandSystem.ExecuteCommand(new AddEntity("entity 1", null));
            //CommandSystem.ExecuteCommand(new AddEntity("entity 2", null));
            //
            //var entityNames = currentScene.AllEntities.Select(e => e.Value.ShownName).ToArray();
            //Assert.AreEqual(2, entityNames.Length);
            //Assert.AreEqual("entity 1", entityNames[0]);
            //Assert.AreEqual("entity 2", entityNames[1]);
            //
            //CommandSystem.UndoCommand();
            //
            //yield return null;
            //
            //entityNames = currentScene.AllEntities.Select(e => e.Value.ShownName).ToArray();
            //Assert.AreEqual(1, entityNames.Length);
            //Assert.AreEqual("entity 1", entityNames[0]);
            //
            //CommandSystem.RedoCommand();
            //
            //entityNames = currentScene.AllEntities.Select(e => e.Value.ShownName).ToArray();
            //Assert.AreEqual(2, entityNames.Length);
            //Assert.AreEqual("entity 1", entityNames[0]);
            //Assert.AreEqual("entity 2", entityNames[1]);
            //
            //CommandSystem.UndoCommand();
            //
            //yield return null;
            //
            //CommandSystem.ExecuteCommand(new AddEntity("child", currentScene.AllEntities.First(e => e.Value.ShownName == "entity 1").Value));
            //
            //CommandSystem.UndoCommand();
            //
            //yield return null;
            //
            //CommandSystem.RedoCommand();
            //
            //var child = currentScene.AllEntities.First(e => e.Value.ShownName == "child");
            //
            //Assert.NotNull(child.Value.Parent);
            //Assert.AreEqual("entity 1", child.Value.Parent.ShownName);
            //Assert.AreEqual("child", child.Value.ShownName);
        }
    }
}
