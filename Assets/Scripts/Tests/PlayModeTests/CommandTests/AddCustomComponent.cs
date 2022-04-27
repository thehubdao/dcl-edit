using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Command;
using Assets.Scripts.EditorState;
using Assets.Scripts.State;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Assets.Scripts.Tests.PlayModeTests.CommandTests
{
    public class AddCustomComponent
    {
        private Guid entId;

        private IEnumerator SetupScene()
        {
            // load scene
            SceneManager.LoadScene(0);
            yield return null;

            // create new dcl scene
            EditorStates.CurrentSceneState.CurrentScene = new DclScene();

            // add new entity to work on
            Command.Command.ExecuteCommand(new AddEntity("myent"));

            // save that entities id
            entId = EditorStates
                .CurrentSceneState
                .CurrentScene
                .AllEntities
                .First(pair => pair.Value.ShownName == "myent")
                .Value
                .Id;
        }


        [UnityTest]
        public IEnumerator AddComponent()
        {
            yield return SetupScene();

            var usedEntity = EditorStates.CurrentSceneState.CurrentScene.GetEntityFormId(entId);

            Command.Command.ExecuteCommand(new AddCustomComponentToEntity(
                entId,
                "myComponent",
                "myComponent",
                new KeyValuePair<Type, string>(typeof(string), "name"),
                new KeyValuePair<Type, string>(typeof(int), "age"),
                new KeyValuePair<Type, string>(typeof(float), "height"),
                new KeyValuePair<Type, string>(typeof(Vector3), "direction")));

            Assert.AreEqual(1, usedEntity.Components.Count);

            var component = usedEntity.Components.First();

            Assert.AreEqual("myComponent", component.NameInCode);
            Assert.AreEqual("myComponent", component.NameOfSlot);

            Assert.AreEqual(4, component.Properties.Count);
            Assert.AreEqual("name", component.Properties[0].PropertyName);
            Assert.AreEqual("age", component.Properties[1].PropertyName);
            Assert.AreEqual("height", component.Properties[2].PropertyName);
            Assert.AreEqual("direction", component.Properties[3].PropertyName);

            Assert.AreEqual(DclComponent.DclComponentProperty.PropertyType.String, component.Properties[0].Type);
            Assert.AreEqual(DclComponent.DclComponentProperty.PropertyType.Int, component.Properties[1].Type);
            Assert.AreEqual(DclComponent.DclComponentProperty.PropertyType.Float, component.Properties[2].Type);
            Assert.AreEqual(DclComponent.DclComponentProperty.PropertyType.Vector3, component.Properties[3].Type);

            var concreteNameProperty = component.Properties[0].GetConcrete<string>();
            Assert.AreEqual("", concreteNameProperty.Value);
            Assert.AreEqual("", concreteNameProperty.FixedValue);
            Assert.AreEqual(false, concreteNameProperty.IsFloating);

            var concreteAgeProperty = component.Properties[1].GetConcrete<int>();
            Assert.AreEqual(0, concreteAgeProperty.Value);
            Assert.AreEqual(0, concreteAgeProperty.FixedValue);
            Assert.AreEqual(false, concreteAgeProperty.IsFloating);

            var concreteHeightProperty = component.Properties[2].GetConcrete<float>();
            Assert.AreEqual(0f, concreteHeightProperty.Value);
            Assert.AreEqual(0f, concreteHeightProperty.FixedValue);
            Assert.AreEqual(false, concreteHeightProperty.IsFloating);

            var concreteDirectionProperty = component.Properties[3].GetConcrete<Vector3>();
            Assert.AreEqual(Vector3.zero, concreteDirectionProperty.Value);
            Assert.AreEqual(Vector3.zero, concreteDirectionProperty.FixedValue);
            Assert.AreEqual(false, concreteDirectionProperty.IsFloating);
            
        }
    }
}
