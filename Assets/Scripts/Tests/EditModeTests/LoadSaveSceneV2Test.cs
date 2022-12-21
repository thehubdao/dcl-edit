using System;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Tests.EditModeTests.TestUtility;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class LoadSaveSceneV2Test
    {
        [Test]
        public void LoadScene()
        {
            var pathState = new MockPathState("simple-load-test");
            var loadSaveSystem = new SceneLoadSaveSystem(pathState);
            var sceneDirectoryState = new SceneDirectoryState();
            var scenePath = pathState.ProjectPath + "/dcl-edit/saves/v2/New Scene.dclscene";

            sceneDirectoryState.directoryPath = scenePath;

            loadSaveSystem.Load(sceneDirectoryState);

            var scene = sceneDirectoryState.currentScene;

            Assert.NotNull(scene);

            Assert.AreEqual("New Scene", scene.name);
            Assert.AreEqual(2, scene.AllEntities.Count());

            var cubeEnt = scene.GetEntityById(Guid.Parse("733338c0-c576-4465-975b-248e9e5e7b63"));

            Assert.NotNull(cubeEnt);
            Assert.AreEqual("Default Cube", cubeEnt.ShownName);
            Assert.AreEqual("Default Cube", cubeEnt.CustomName);
            Assert.AreEqual(Guid.Parse("733338c0-c576-4465-975b-248e9e5e7b63"), cubeEnt.Id);
            Assert.AreEqual(false, cubeEnt.IsExposed);

            var cubeEntTransform = cubeEnt.GetTransformComponent();

            Assert.NotNull(cubeEntTransform);

            // position
            Assert.AreEqual(new Vector3(8, 0.5f, 3), cubeEntTransform.Position.Value);

            // rotation
            Assert.AreEqual(new Quaternion(0, 0, 0, 1), cubeEntTransform.Rotation.Value);

            // scale
            Assert.AreEqual(new Vector3(1, 1, 1), cubeEntTransform.Scale.Value);

            // BoxShape
            var cubeEntBoxShape = cubeEnt.GetComponentByName("BoxShape");

            Assert.NotNull(cubeEntBoxShape);

            Assert.AreEqual("Shape", cubeEntBoxShape.NameOfSlot);
        }

        [Test]
        public void LoadedFilePaths()
        {
            var pathState = new MockPathState("simple-load-test", true);
            var loadSaveSystem = new SceneLoadSaveSystem(pathState);
            var scenePath = pathState.ProjectPath + "/dcl-edit/saves/v2/New Scene.dclscene";

            // Load scene, add a entity, save, remove a entity, and save again. This should result in the original scene
            {
                var sceneDirectoryState = new SceneDirectoryState
                {
                    directoryPath = scenePath
                };

                loadSaveSystem.Load(sceneDirectoryState);

                var scene = sceneDirectoryState.currentScene;

                Assert.NotNull(scene);

                var cubeEnt = scene.GetEntityById(Guid.Parse("733338c0-c576-4465-975b-248e9e5e7b63"));
                Assert.NotNull(cubeEnt);

                var sphereEnt = scene.GetEntityById(Guid.Parse("84089323-69df-4d27-b0da-a51afcdd88bf"));
                Assert.NotNull(sphereEnt);

                // Add entity without command. TODO: Should be done differently
                var newEntityId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
                scene.AddEntity(new DclEntity(newEntityId, "New Entity"));

                var newEntity = scene.GetEntityById(newEntityId);
                Assert.NotNull(newEntity);

                loadSaveSystem.Save(sceneDirectoryState);

                // Delete entity without command. TODO: Should be done differently
                scene.RemoveEntity(newEntityId);

                newEntity = scene.GetEntityById(newEntityId);
                Assert.Null(newEntity);

                loadSaveSystem.Save(sceneDirectoryState);
            }

            // Load scene again and observe, that there are only two entities
            {
                var sceneDirectoryState = new SceneDirectoryState
                {
                    directoryPath = scenePath
                };

                loadSaveSystem.Load(sceneDirectoryState);

                var scene = sceneDirectoryState.currentScene;

                Assert.NotNull(scene);

                var newEntityId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
                var newEntity = scene.GetEntityById(newEntityId);
                Assert.Null(newEntity);
            }
        }
    }
}
