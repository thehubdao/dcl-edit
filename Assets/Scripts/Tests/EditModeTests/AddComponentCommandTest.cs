using System;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using NUnit.Framework;
using UnityEngine;
using static Assets.Scripts.SceneState.DclComponent.DclComponentProperty;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class AddComponentCommandTest
    {
        DclScene scene;

        DclEntity testEntity1;
        DclEntity testEntity2;
        DclEntity testEntity3;

        CommandFactorySystem commandFactory;

        EditorEvents editorEvents;

        [SetUp]
        public void Setup()
        {
            scene = new DclScene();

            // Create three test entities where entity 3 it a child of entity 2
            testEntity1 = new DclEntity(Guid.NewGuid(), "Entity 1");
            testEntity2 = new DclEntity(Guid.NewGuid(), "Entity 2");
            testEntity3 = new DclEntity(Guid.NewGuid(), "Entity 3", testEntity2.Id);

            scene.AddEntity(testEntity1);
            scene.AddEntity(testEntity2);
            scene.AddEntity(testEntity3);

            commandFactory = new CommandFactorySystem();

            editorEvents = new EditorEvents();
        }


        [Test]
        public void AddShapeComponent()
        {
            // create a command to add a SphereShape to test entity 1
            var componentDefinition = new DclComponent.ComponentDefinition("SphereShape", "Shape");
            var command = commandFactory.CreateAddComponent(testEntity1.Id, componentDefinition);

            // execute the command
            command.Do(scene, editorEvents);

            // check that the entity has the component
            Assert.IsTrue(testEntity1.HasComponent(componentDefinition.NameInCode));
        }

        [Test]
        public void AddShapeComponentToChild()
        {
            // create a command to add a SphereShape to test entity 3
            var componentDefinition = new DclComponent.ComponentDefinition("SphereShape", "Shape");
            var command = commandFactory.CreateAddComponent(testEntity3.Id, componentDefinition);

            // execute the command
            command.Do(scene, editorEvents);

            // check that the entity has the component
            Assert.IsTrue(testEntity3.HasComponent(componentDefinition.NameInCode));
        }

        [Test]
        public void UndoAndRedo()
        {
            // create a command to add a SphereShape to test entity 1
            var componentDefinition = new DclComponent.ComponentDefinition("SphereShape", "Shape");
            var command = commandFactory.CreateAddComponent(testEntity1.Id, componentDefinition);

            // execute the command
            command.Do(scene, editorEvents);

            // check that the entity has the component
            Assert.IsTrue(testEntity1.HasComponent(componentDefinition.NameInCode));

            // undo the command
            command.Undo(scene, editorEvents);

            // check that the entity does not have the component
            Assert.IsFalse(testEntity1.HasComponent(componentDefinition.NameInCode));

            // redo the command
            command.Do(scene, editorEvents);

            // check that the entity has the component
            Assert.IsTrue(testEntity1.HasComponent(componentDefinition.NameInCode));
        }

        [Test]
        public void TryAddingComponentWithOccupiedSlot()
        {
            // create a command to add a SphereShape to test entity 1
            var componentDefinition = new DclComponent.ComponentDefinition("SphereShape", "Shape");
            var command = commandFactory.CreateAddComponent(testEntity1.Id, componentDefinition);

            // execute the command
            command.Do(scene, editorEvents);

            // check that the entity has the component
            Assert.IsTrue(testEntity1.HasComponent(componentDefinition.NameInCode));

            // create a command to add a BoxShape to test entity 1
            var componentDefinition2 = new DclComponent.ComponentDefinition("BoxShape", "Shape");
            var command2 = commandFactory.CreateAddComponent(testEntity1.Id, componentDefinition2);

            // execute the command
            Assert.Throws<Exception>(() => command2.Do(scene, editorEvents));

            // check that the entity still has the sphere shape component
            Assert.IsTrue(testEntity1.HasComponent(componentDefinition.NameInCode));

            // check that the entity does not have the box shape component
            Assert.IsFalse(testEntity1.HasComponent(componentDefinition2.NameInCode));
        }

        [Test]
        public void AddComponentWithProperties()
        {
            // create a command to add a MoveUp component to test entity 1
            var componentDefinition = new DclComponent.ComponentDefinition(
                "MoveUp",
                "MoveUp",
                new PropertyDefinition("speed", PropertyType.Float, 1f),
                new PropertyDefinition("distance", PropertyType.Float, 1f),
                new PropertyDefinition("direction", PropertyType.Vector3, new Vector3(0, 1, 0))
            );
            var command = commandFactory.CreateAddComponent(testEntity1.Id, componentDefinition);

            // execute the command
            command.Do(scene, editorEvents);

            // check that the entity has the component
            Assert.IsTrue(testEntity1.HasComponent(componentDefinition.NameInCode));

            // check that the component has the correct properties
            var component = testEntity1.GetComponentByName(componentDefinition.NameInCode);

            var speedProperty = component.GetPropertyByName("speed");
            Assert.AreEqual("speed", speedProperty.PropertyName);
            Assert.AreEqual(PropertyType.Float, speedProperty.Type);
            Assert.AreEqual(1, speedProperty.GetConcrete<float>().Value);

            var distanceProperty = component.GetPropertyByName("distance");
            Assert.AreEqual("distance", distanceProperty.PropertyName);
            Assert.AreEqual(PropertyType.Float, distanceProperty.Type);
            Assert.AreEqual(1, distanceProperty.GetConcrete<float>().Value);

            var directionProperty = component.GetPropertyByName("direction");
            Assert.AreEqual("direction", directionProperty.PropertyName);
            Assert.AreEqual(PropertyType.Vector3, directionProperty.Type);
            Assert.AreEqual(new Vector3(0, 1, 0), directionProperty.GetConcrete<Vector3>().Value);
        }

        [Test]
        public void AddMultipleComponents()
        {
            // create a command to add a SphereShape to test entity 1
            var componentDefinition = new DclComponent.ComponentDefinition("SphereShape", "Shape");
            var command = commandFactory.CreateAddComponent(testEntity1.Id, componentDefinition);

            // execute the command
            command.Do(scene, editorEvents);

            // create a command to add a MoveUp component to test entity 1
            var componentDefinition2 = new DclComponent.ComponentDefinition(
                "MoveUp",
                "MoveUp",
                new PropertyDefinition("speed", PropertyType.Float, 1f),
                new PropertyDefinition("distance", PropertyType.Float, 1f),
                new PropertyDefinition("direction", PropertyType.Vector3, new Vector3(0, 1, 0))
            );
            var command2 = commandFactory.CreateAddComponent(testEntity1.Id, componentDefinition2);

            // execute the command
            command2.Do(scene, editorEvents);

            // check that the entity has the sphere shape component
            Assert.IsTrue(testEntity1.HasComponent(componentDefinition.NameInCode));

            // check that the entity has the move up component
            Assert.IsTrue(testEntity1.HasComponent(componentDefinition2.NameInCode));
        }

        [Test]
        public void DependentCommandUndoRedo()
        {
            // create a command to add a MoveUp component to test entity 1
            var componentDefinition = new DclComponent.ComponentDefinition(
                "MoveUp",
                "MoveUp",
                new PropertyDefinition("speed", PropertyType.Float, 1f),
                new PropertyDefinition("distance", PropertyType.Float, 1f),
                new PropertyDefinition("direction", PropertyType.Vector3, new Vector3(0, 1, 0))
            );
            var command = commandFactory.CreateAddComponent(testEntity1.Id, componentDefinition);

            // execute the command
            command.Do(scene, editorEvents);

            // create a dependent command to change the speed property of the MoveUp component
            var command2 = commandFactory.CreateChangePropertyCommand<float>(new DclPropertyIdentifier(testEntity1.Id, "MoveUp", "speed"), 1, 2);

            // execute the command
            command2.Do(scene, editorEvents);

            // check that the speed property has the correct value
            var component = testEntity1.GetComponentByName(componentDefinition.NameInCode);
            var speedProperty = component.GetPropertyByName("speed");
            Assert.AreEqual(2, speedProperty.GetConcrete<float>().Value);

            // undo the command
            command2.Undo(scene, editorEvents);

            // undo the other command
            command.Undo(scene, editorEvents);

            // check that the entity does not have the component
            Assert.IsFalse(testEntity1.HasComponent(componentDefinition.NameInCode));

            // redo the command
            command.Do(scene, editorEvents);

            // check that the entity has the component
            Assert.IsTrue(testEntity1.HasComponent(componentDefinition.NameInCode));

            // check that speed has the default value
            component = testEntity1.GetComponentByName(componentDefinition.NameInCode);
            speedProperty = component.GetPropertyByName("speed");
            Assert.AreEqual(1, speedProperty.GetConcrete<float>().Value);

            // redo the dependent command
            command2.Do(scene, editorEvents);

            // check that the speed property has the correct value
            component = testEntity1.GetComponentByName(componentDefinition.NameInCode);
            speedProperty = component.GetPropertyByName("speed");
            Assert.AreEqual(2, speedProperty.GetConcrete<float>().Value);
        }
    }
}
