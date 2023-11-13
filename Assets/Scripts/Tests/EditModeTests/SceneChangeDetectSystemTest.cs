using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts.Tests.EditModeTests.TestUtility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class SceneChangeDetectSystemTest
    {
        // Dependencies
        CommandSystem commandSystem;
        CommandFactorySystem commandFactorySystem;
        EditorEvents editorEvents;
        ISceneManagerSystem sceneManagerSystem;
        IMenuBarSystem menuBarSystem;
        SceneChangeDetectSystem sceneChangeDetectSystem;

        [SetUp]
        public void Setup()
        {
            commandSystem = new CommandSystem();
            commandFactorySystem = new CommandFactorySystem();
            editorEvents = new EditorEvents();
            sceneManagerSystem = new MockSceneManagerSystem();
            menuBarSystem = new MockMenuBarSystem();
            sceneChangeDetectSystem = new SceneChangeDetectSystem();
            sceneChangeDetectSystem.Construct(new MockSceneViewSystem());
            commandSystem.Construct(commandFactorySystem, editorEvents, sceneManagerSystem, menuBarSystem, sceneChangeDetectSystem);
        }

        [Test]
        public void TestUndoAndRedo()
        {
            // Scene was just loaded, no changes
            Assert.False(sceneChangeDetectSystem.HasSceneChanged());

            // Run two new commands
            //commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateTranslateTransform(Guid.NewGuid(), Vector3.zero, Vector3.one));
            //commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateTranslateTransform(Guid.NewGuid(), Vector3.one, Vector3.zero));
            Assert.True(sceneChangeDetectSystem.HasSceneChanged());

            // Undo commands until back in saved state
            commandSystem.UndoCommand();
            Assert.True(sceneChangeDetectSystem.HasSceneChanged());
            commandSystem.UndoCommand();
            Assert.False(sceneChangeDetectSystem.HasSceneChanged());

            // Redo commands to return to last state
            commandSystem.RedoCommand();
            commandSystem.RedoCommand();
            Assert.True(sceneChangeDetectSystem.HasSceneChanged());
        }

        [Test]
        public void TestOverwritingCommandHistory()
        {
            // Execute new command, save and execute another command.
            //commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateTranslateTransform(Guid.NewGuid(), Vector3.zero, Vector3.one));
            //commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateTranslateTransform(Guid.NewGuid(), Vector3.one, Vector3.zero));
            commandSystem.UndoCommand();
            sceneChangeDetectSystem.RememberCurrentState();
            Assert.False(sceneChangeDetectSystem.HasSceneChanged());
            //commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateTranslateTransform(Guid.NewGuid(), Vector3.one, Vector3.zero));
            Assert.True(sceneChangeDetectSystem.HasSceneChanged());

            // Return to the command before the scene was saved. That makes it so that the saved state lies in the future.
            // Then overwrite the command history by executing a new command
            commandSystem.UndoCommand();
            Assert.False(sceneChangeDetectSystem.HasSceneChanged());
            commandSystem.UndoCommand();
            //commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateTranslateTransform(Guid.NewGuid(), Vector3.one, Vector3.zero));
            Assert.True(sceneChangeDetectSystem.HasSceneChanged());
        }

        [Test]
        public void TestNonModifyingCommand()
        {
            // Save and execute a non scene modifying command
            sceneChangeDetectSystem.RememberCurrentState();
            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangeSelection(Guid.NewGuid(), new List<Guid>(), Guid.NewGuid(), new List<Guid>()));
            Assert.False(sceneChangeDetectSystem.HasSceneChanged());
            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangeSelection(Guid.NewGuid(), new List<Guid>(), Guid.NewGuid(), new List<Guid>()));
            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangeSelection(Guid.NewGuid(), new List<Guid>(), Guid.NewGuid(), new List<Guid>()));
            Assert.False(sceneChangeDetectSystem.HasSceneChanged());
            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangeSelection(Guid.NewGuid(), new List<Guid>(), Guid.NewGuid(), new List<Guid>()));
            commandSystem.UndoCommand();
            commandSystem.UndoCommand();
            commandSystem.UndoCommand();
            Assert.False(sceneChangeDetectSystem.HasSceneChanged());
        }
    }
}