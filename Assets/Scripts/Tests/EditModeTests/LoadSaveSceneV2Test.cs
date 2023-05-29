using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Tests.EditModeTests.TestUtility;
using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class LoadSaveSceneV2Test
    {
        [Test]
        public void LoadScene()
        {
            var pathState = new MockPathState("simple-load-test");
            var loadSaveSystem = new SceneLoadSaveSystem();
            loadSaveSystem.Construct(null, new FileUpgraderSystem(), new MockSceneViewSystem(), new MockSceneChangeDetectSystem());
            var sceneDirectoryState = new SceneDirectoryState();
            var scenePath = pathState.ProjectPath + "/dcl-edit/saves/v2/New Scene.dclscene";

            sceneDirectoryState.directoryPath = scenePath;

            loadSaveSystem.Load(sceneDirectoryState);

            var scene = sceneDirectoryState.currentScene;

            Assert.NotNull(scene);

            Assert.AreEqual("New Scene", sceneDirectoryState.name);
            Assert.AreEqual(sceneDirectoryState.id, Guid.Parse("e8cafbb9-54f5-4064-8982-6cf86db3506a"));
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
            Assert.AreEqual(new Vector3(8, 0.5f, 3), cubeEntTransform.position.Value);

            // rotation
            Assert.AreEqual(new Quaternion(0, 0, 0, 1), cubeEntTransform.rotation.Value);

            // scale
            Assert.AreEqual(new Vector3(1, 1, 1), cubeEntTransform.scale.Value);

            // BoxShape
            var cubeEntBoxShape = cubeEnt.GetComponentByName("BoxShape");

            Assert.NotNull(cubeEntBoxShape);

            Assert.AreEqual("Shape", cubeEntBoxShape.NameOfSlot);
        }

        [Test]
        public void LoadedFilePaths()
        {
            SceneManagerState sceneManagerState = new SceneManagerState();
            MockPathState pathState = new MockPathState("simple-load-test", true);
            SceneLoadSaveSystem loadSaveSystem = new SceneLoadSaveSystem();
            SceneSettingState sceneSettingState = new SceneSettingState();
            CheckVersionSystem checkVersionSystem = new CheckVersionSystem();
            WorkspaceSaveSystem workspaceSaveSystem = new WorkspaceSaveSystem();
            TypeScriptGenerationSystem typeScriptGenerationSystem = new TypeScriptGenerationSystem();
            ISceneViewSystem sceneViewSystem = new MockSceneViewSystem();
            MenuBarSystem menuBarSystem = new MenuBarSystem();
            SceneManagerSystem sceneManagerSystem = new SceneManagerSystem();
            EditorEvents editorEvents = new EditorEvents();
            MenuBarState menuBarState = new MenuBarState();
            SettingsSystem settingsSystem = new SettingsSystem(editorEvents);


            menuBarSystem.Construct(editorEvents, menuBarState);
            sceneSettingState.Construct(pathState, sceneManagerState);
            sceneManagerSystem.Construct(
                sceneManagerState,
                pathState,
                loadSaveSystem,
                loadSaveSystem,
                sceneSettingState,
                checkVersionSystem,
                workspaceSaveSystem,
                typeScriptGenerationSystem,
                sceneViewSystem,
                menuBarSystem,
                settingsSystem);
            loadSaveSystem.Construct(null, new FileUpgraderSystem(), sceneViewSystem, new MockSceneChangeDetectSystem());

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

        [Test]
        public void SaveScene()
        {
            // Setup dependencies
            var pathState = new MockPathState("simple-load-test", true);
            var loadSaveSystem = new SceneLoadSaveSystem();

            // Create scene directory state
            string scenePath = pathState.ProjectPath + "/dcl-edit/saves/v2/New Scene.dclscene";
            Guid sceneId = Guid.NewGuid();
            SceneDirectoryState sceneDirectoryState = new SceneDirectoryState(scenePath, sceneId);

            // Load scene from testing repository
            loadSaveSystem.Load(sceneDirectoryState);

            // Change scene ID
            sceneDirectoryState.id = sceneId;

            // Change names of existing entities
            DclEntity cube = sceneDirectoryState.currentScene.GetEntityById(Guid.Parse("733338c0-c576-4465-975b-248e9e5e7b63"));
            cube.CustomName = "Default Cube Renamed";
            DclEntity sphere = sceneDirectoryState.currentScene.GetEntityById(Guid.Parse("84089323-69df-4d27-b0da-a51afcdd88bf"));
            sphere.CustomName = "Default Sphere Renamed";

            // Create new entities
            Guid entity1Id = Guid.NewGuid();
            Guid entity2Id = Guid.NewGuid();
            DclEntity entity1 = new DclEntity(entity1Id, "Entity1");
            DclEntity entity2 = new DclEntity(entity2Id, "Entity2");

            // Add new entities to scene
            sceneDirectoryState.currentScene.AddEntity(entity1);
            sceneDirectoryState.currentScene.AddEntity(entity2);

            // Save to filesystem
            loadSaveSystem.Save(sceneDirectoryState);

            // Load from filesystem
            SceneDirectoryState loadedSceneDirectoryState = new SceneDirectoryState(scenePath, Guid.NewGuid());
            loadSaveSystem.Load(loadedSceneDirectoryState);

            // Run tests
            Assert.AreEqual(loadedSceneDirectoryState.id, sceneId);
            Assert.AreEqual(loadedSceneDirectoryState.name, "New Scene");
            Assert.NotNull(loadedSceneDirectoryState.currentScene);
            DclEntity loadedEntity1 = loadedSceneDirectoryState.currentScene.GetEntityById(entity1Id);
            Assert.NotNull(loadedEntity1);
            Assert.AreEqual(loadedEntity1.Id, entity1Id);
            Assert.AreEqual(loadedEntity1.CustomName, "Entity1");
            DclEntity loadedEntity2 = loadedSceneDirectoryState.currentScene.GetEntityById(entity2Id);
            Assert.NotNull(loadedEntity2);
            Assert.AreEqual(loadedEntity2.Id, entity2Id);
            Assert.AreEqual(loadedEntity2.CustomName, "Entity2");
            DclEntity loadedCube = loadedSceneDirectoryState.currentScene.GetEntityById(Guid.Parse("733338c0-c576-4465-975b-248e9e5e7b63"));
            Assert.NotNull(loadedCube);
            Assert.AreEqual(loadedCube.Id, Guid.Parse("733338c0-c576-4465-975b-248e9e5e7b63"));
            Assert.AreEqual(loadedCube.CustomName, "Default Cube Renamed");
            DclEntity loadedSphere = loadedSceneDirectoryState.currentScene.GetEntityById(Guid.Parse("84089323-69df-4d27-b0da-a51afcdd88bf"));
            Assert.NotNull(loadedSphere);
            Assert.AreEqual(loadedSphere.Id, Guid.Parse("84089323-69df-4d27-b0da-a51afcdd88bf"));
            Assert.AreEqual(loadedSphere.CustomName, "Default Sphere Renamed");
        }
    }
}
