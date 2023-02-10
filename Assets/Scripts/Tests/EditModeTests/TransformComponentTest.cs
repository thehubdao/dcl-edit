using System;
using Assets.Scripts.SceneState;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class TransformComponentTest
    {
        [Test]
        public void GetProperties()
        {
            var testTransform = new DclTransformComponent(new Vector3(1, 2, 3), Quaternion.Euler(4, 5, 6), new Vector3(7, 8, 9));

            // get properties from build in shortcuts
            Assert.AreEqual(new Vector3(1, 2, 3), testTransform.Position.Value);
            Assert.AreEqual(Quaternion.Euler(4, 5, 6), testTransform.Rotation.Value);
            Assert.AreEqual(new Vector3(7, 8, 9), testTransform.Scale.Value);

            // get properties from raw component
            var testComponent = (DclComponent) testTransform;

            Assert.AreEqual(new Vector3(1, 2, 3), testComponent.GetPropertyByName("position").GetConcrete<Vector3>().Value);
            Assert.AreEqual(Quaternion.Euler(4, 5, 6), testComponent.GetPropertyByName("rotation").GetConcrete<Quaternion>().Value);
            Assert.AreEqual(new Vector3(7, 8, 9), testComponent.GetPropertyByName("scale").GetConcrete<Vector3>().Value);
        }

        [Test]
        public void SetProperties()
        {
            var testTransform = new DclTransformComponent(new Vector3(1, 2, 3), Quaternion.Euler(4, 5, 6), new Vector3(7, 8, 9));

            // set properties
            testTransform.Position.SetFloatingValue(new Vector3(10, 11, 12));
            testTransform.Rotation.SetFloatingValue(Quaternion.Euler(13, 14, 15));
            testTransform.Scale.SetFloatingValue(new Vector3(16, 17, 18));

            // get properties from build in shortcuts
            Assert.AreEqual(new Vector3(10, 11, 12), testTransform.Position.Value);
            Assert.AreEqual(Quaternion.Euler(13, 14, 15), testTransform.Rotation.Value);
            Assert.AreEqual(new Vector3(16, 17, 18), testTransform.Scale.Value);

            // get properties from raw component
            var testComponent = (DclComponent) testTransform;

            Assert.AreEqual(new Vector3(10, 11, 12), testComponent.GetPropertyByName("position").GetConcrete<Vector3>().Value);
            Assert.AreEqual(Quaternion.Euler(13, 14, 15), testComponent.GetPropertyByName("rotation").GetConcrete<Quaternion>().Value);
            Assert.AreEqual(new Vector3(16, 17, 18), testComponent.GetPropertyByName("scale").GetConcrete<Vector3>().Value);
        }

        [Test]
        public void TransformFromNormalComponent()
        {
            var sourceTransform = new DclTransformComponent(new Vector3(1, 2, 3), Quaternion.Euler(4, 5, 6), new Vector3(7, 8, 9));
            var sourceComponent = (DclComponent) sourceTransform;

            var testTransform = new DclTransformComponent(sourceComponent);

            // get properties from build in shortcuts
            Assert.AreEqual(new Vector3(1, 2, 3), testTransform.Position.Value);
            Assert.AreEqual(Quaternion.Euler(4, 5, 6), testTransform.Rotation.Value);
            Assert.AreEqual(new Vector3(7, 8, 9), testTransform.Scale.Value);

            // set properties
            testTransform.Position.SetFloatingValue(new Vector3(10, 11, 12));
            testTransform.Rotation.SetFloatingValue(Quaternion.Euler(13, 14, 15));
            testTransform.Scale.SetFloatingValue(new Vector3(16, 17, 18));

            // get properties from source transform
            Assert.AreEqual(new Vector3(10, 11, 12), sourceTransform.Position.Value);
            Assert.AreEqual(Quaternion.Euler(13, 14, 15), sourceTransform.Rotation.Value);
            Assert.AreEqual(new Vector3(16, 17, 18), sourceTransform.Scale.Value);
        }

        [Test]
        public void GlobalPositionNoParent()
        {
            var testScene = new DclScene();

            var testEntity = new DclEntity(Guid.NewGuid(), "testEntity");
            testScene.AddEntity(testEntity);

            var testTransform = new DclTransformComponent(new Vector3(1, 2, 3), Quaternion.Euler(4, 5, 6), new Vector3(7, 8, 9));
            testEntity.AddComponent(testTransform);

            Assert.AreEqual(new Vector3(1, 2, 3), testTransform.GlobalPosition);
        }

        [Test]
        public void GlobalPositionWithParent()
        {
            var testScene = new DclScene();

            var parentEntity = new DclEntity(Guid.NewGuid(), "testEntity");
            testScene.AddEntity(parentEntity);

            var parentTransform = new DclTransformComponent(new Vector3(1, 2, 3), Quaternion.identity, Vector3.one);
            parentEntity.AddComponent(parentTransform);

            var testEntity = new DclEntity(Guid.NewGuid(), "testEntity", parentEntity.Id);
            testScene.AddEntity(testEntity);

            var testTransform = new DclTransformComponent(new Vector3(10, 11, 12), Quaternion.identity, Vector3.one);
            testEntity.AddComponent(testTransform);

            Assert.AreEqual(new Vector3(11, 13, 15), testTransform.GlobalPosition);

            parentTransform.Scale.SetFloatingValue(new Vector3(2, 2, 2));

            Assert.AreEqual(new Vector3(21, 24, 27), testTransform.GlobalPosition);

            parentTransform.Rotation.SetFloatingValue(Quaternion.Euler(0, -90, 0));

            Assert.AreEqual(new Vector3(-23, 24, 23), testTransform.GlobalPosition);
        }
    }
}
