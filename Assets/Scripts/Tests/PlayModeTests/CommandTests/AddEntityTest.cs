using System.Collections;
using System.Linq;
using Assets.Scripts.Command;
using Assets.Scripts.EditorState;
using Assets.Scripts.State;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Tests.PlayModeTests.CommandTests
{
    public class AddEntityTest
    {
        private IEnumerator SetupScene()
        {
            SceneManager.LoadScene(0);
            yield return null;
            // create new dcl scene
            EditorStates.CurrentSceneState.CurrentScene = new DclScene();
        }


        [UnityTest]
        public IEnumerator AddingSingleEntity()
        {
            yield return SetupScene();

            var currentScene = EditorStates.CurrentSceneState.CurrentScene;

            Command.Command.ExecuteCommand(new AddEntity("cool name", null));
            Assert.AreEqual(1, currentScene.AllEntities.Count);

            var entity = currentScene.AllEntities.First(e => e.Value.ShownName == "cool name");
            Assert.AreEqual("cool name", entity.Value.ShownName);

            Assert.AreEqual(currentScene, entity.Value.Scene);
        }

        [UnityTest]
        public IEnumerator AddingEntityWithParent()
        {
            yield return SetupScene();

            var currentScene = EditorStates.CurrentSceneState.CurrentScene;

            Command.Command.ExecuteCommand(new AddEntity("parent", null));
            var parent = currentScene.AllEntities.First(e => e.Value.ShownName == "parent");
            Command.Command.ExecuteCommand(new AddEntity("child", parent.Value));
            var child = currentScene.AllEntities.First(e => e.Value.ShownName == "child");

            Assert.AreEqual(2, currentScene.AllEntities.Count);
            Assert.AreEqual(parent.Value, child.Value.Parent);

            Assert.AreEqual(currentScene, parent.Value.Scene);
            Assert.AreEqual(currentScene, child.Value.Scene);
        }

        [UnityTest]
        public IEnumerator AddEntityHistory()
        {
            yield return SetupScene();

            var currentScene = EditorStates.CurrentSceneState.CurrentScene;

            Command.Command.ExecuteCommand(new AddEntity("entity 1", null));
            Command.Command.ExecuteCommand(new AddEntity("entity 2", null));

            var entityNames = currentScene.AllEntities.Select(e => e.Value.ShownName).ToArray();
            Assert.AreEqual(2, entityNames.Length);
            Assert.AreEqual("entity 1", entityNames[0]);
            Assert.AreEqual("entity 2", entityNames[1]);

            Command.Command.UndoCommand();

            yield return null;

            entityNames = currentScene.AllEntities.Select(e => e.Value.ShownName).ToArray();
            Assert.AreEqual(1, entityNames.Length);
            Assert.AreEqual("entity 1", entityNames[0]);

            Command.Command.RedoCommand();

            entityNames = currentScene.AllEntities.Select(e => e.Value.ShownName).ToArray();
            Assert.AreEqual(2, entityNames.Length);
            Assert.AreEqual("entity 1", entityNames[0]);
            Assert.AreEqual("entity 2", entityNames[1]);

            Command.Command.UndoCommand();

            yield return null;

            Command.Command.ExecuteCommand(new AddEntity("child", currentScene.AllEntities.First(e => e.Value.ShownName == "entity 1").Value));

            Command.Command.UndoCommand();

            yield return null;

            Command.Command.RedoCommand();

            var child = currentScene.AllEntities.First(e => e.Value.ShownName == "child");

            Assert.NotNull(child.Value.Parent);
            Assert.AreEqual("entity 1", child.Value.Parent.ShownName);
            Assert.AreEqual("child", child.Value.ShownName);
        }
    }
}
