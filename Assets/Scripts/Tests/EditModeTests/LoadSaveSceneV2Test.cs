using System;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using Assets.Scripts.Tests.EditModeTests.TestUtility;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class LoadSaveSceneV2Test
    {
        [Test]
        public void LoadScene()
        {
            var pathState = new MockPathState("simple-load-test");
            var lsSystem = new SceneLoadSaveSystem(pathState);
            var sceneDirectoryState = new SceneDirectoryState();
            var scenePath = pathState.ProjectPath + "/dcl-edit/saves/v2/New Scene.dclscene";

            sceneDirectoryState.DirectoryPath = scenePath;

            lsSystem.Load(sceneDirectoryState);

            var scene = sceneDirectoryState.CurrentScene;

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
    }
}
