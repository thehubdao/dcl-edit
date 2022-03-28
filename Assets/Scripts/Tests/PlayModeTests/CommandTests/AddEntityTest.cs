using System.Collections;
using System.Linq;
using Assets.Scripts.Command;
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
        private DclScene _scene;
        
        
        [SetUp]
        public void SetUp()
        {
            _scene = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/editor/Scene.prefab")).GetComponent<DclScene>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_scene);
        }
        
        
        [Test]
        public void AddingSingleEntity()
        {
            Command.Command.ExecuteCommand(new AddEntity(_scene, "cool name", null));
            Assert.AreEqual(1, _scene.AllEntities.Length);

            var entity = _scene.AllEntities.First(e => e.ShownName == "cool name");
            Assert.AreEqual("cool name", entity.ShownName);

            Assert.AreEqual(_scene,entity.Scene);
        }

        [Test]
        public void AddingEntityWithParent()
        {
            Command.Command.ExecuteCommand(new AddEntity(_scene, "parent", null));
            var parent = _scene.AllEntities.First(e => e.ShownName == "parent");
            Command.Command.ExecuteCommand(new AddEntity(_scene, "child", parent));
            var child = _scene.AllEntities.First(e => e.ShownName == "child");

            Assert.AreEqual(2, _scene.AllEntities.Length);
            Assert.AreEqual(parent, child.Parent);

            Assert.AreEqual(_scene, parent.Scene);
            Assert.AreEqual(_scene, child.Scene);
        }

        [UnityTest]
        public IEnumerator AddEntityHistory()
        {
            Command.Command.ExecuteCommand(new AddEntity(_scene, "entity 1", null));
            Command.Command.ExecuteCommand(new AddEntity(_scene, "entity 2", null));

            var entityNames = _scene.AllEntities.Select(e => e.ShownName).ToArray();
            Assert.AreEqual(2,entityNames.Length);
            Assert.AreEqual("entity 1", entityNames[0]);
            Assert.AreEqual("entity 2", entityNames[1]);

            Command.Command.UndoCommand();
            
            yield return null;

            entityNames = _scene.AllEntities.Select(e => e.ShownName).ToArray();
            Assert.AreEqual(1, entityNames.Length);
            Assert.AreEqual("entity 1", entityNames[0]);

            Command.Command.RedoCommand();

            entityNames = _scene.AllEntities.Select(e => e.ShownName).ToArray();
            Assert.AreEqual(2, entityNames.Length);
            Assert.AreEqual("entity 1", entityNames[0]);
            Assert.AreEqual("entity 2", entityNames[1]);

            Command.Command.UndoCommand();

            yield return null;

            Command.Command.ExecuteCommand(new AddEntity(_scene, "child", _scene.AllEntities.First(e => e.ShownName == "entity 1")));

            Command.Command.UndoCommand();

            yield return null;

            Command.Command.RedoCommand();
            
            var child = _scene.AllEntities.First(e => e.ShownName == "child");

            Assert.NotNull(child.Parent);
            Assert.AreEqual("entity 1", child.Parent.ShownName);
            Assert.AreEqual("child", child.ShownName);
        }
    }
}
