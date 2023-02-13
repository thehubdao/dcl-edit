using System;
using Assets.Scripts.SceneState;
using Assets.Scripts.Tests.EditModeTests.TestUtility;
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
            CustomAsserts.AreEqualVector(new Vector3(1, 2, 3), testTransform.Position.Value);
            Assert.AreEqual(Quaternion.Euler(4, 5, 6), testTransform.Rotation.Value);
            CustomAsserts.AreEqualVector(new Vector3(7, 8, 9), testTransform.Scale.Value);

            // get properties from raw component
            var testComponent = (DclComponent) testTransform;

            CustomAsserts.AreEqualVector(new Vector3(1, 2, 3), testComponent.GetPropertyByName("position").GetConcrete<Vector3>().Value);
            Assert.AreEqual(Quaternion.Euler(4, 5, 6), testComponent.GetPropertyByName("rotation").GetConcrete<Quaternion>().Value);
            CustomAsserts.AreEqualVector(new Vector3(7, 8, 9), testComponent.GetPropertyByName("scale").GetConcrete<Vector3>().Value);
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
            CustomAsserts.AreEqualVector(new Vector3(10, 11, 12), testTransform.Position.Value);
            Assert.AreEqual(Quaternion.Euler(13, 14, 15), testTransform.Rotation.Value);
            CustomAsserts.AreEqualVector(new Vector3(16, 17, 18), testTransform.Scale.Value);

            // get properties from raw component
            var testComponent = (DclComponent) testTransform;

            CustomAsserts.AreEqualVector(new Vector3(10, 11, 12), testComponent.GetPropertyByName("position").GetConcrete<Vector3>().Value);
            Assert.AreEqual(Quaternion.Euler(13, 14, 15), testComponent.GetPropertyByName("rotation").GetConcrete<Quaternion>().Value);
            CustomAsserts.AreEqualVector(new Vector3(16, 17, 18), testComponent.GetPropertyByName("scale").GetConcrete<Vector3>().Value);
        }

        [Test]
        public void TransformFromNormalComponent()
        {
            var sourceTransform = new DclTransformComponent(new Vector3(1, 2, 3), Quaternion.Euler(4, 5, 6), new Vector3(7, 8, 9));
            var sourceComponent = (DclComponent) sourceTransform;

            var testTransform = new DclTransformComponent(sourceComponent);

            // get properties from build in shortcuts
            CustomAsserts.AreEqualVector(new Vector3(1, 2, 3), testTransform.Position.Value);
            Assert.AreEqual(Quaternion.Euler(4, 5, 6), testTransform.Rotation.Value);
            CustomAsserts.AreEqualVector(new Vector3(7, 8, 9), testTransform.Scale.Value);

            // set properties
            testTransform.Position.SetFloatingValue(new Vector3(10, 11, 12));
            testTransform.Rotation.SetFloatingValue(Quaternion.Euler(13, 14, 15));
            testTransform.Scale.SetFloatingValue(new Vector3(16, 17, 18));

            // get properties from source transform
            CustomAsserts.AreEqualVector(new Vector3(10, 11, 12), sourceTransform.Position.Value);
            Assert.AreEqual(Quaternion.Euler(13, 14, 15), sourceTransform.Rotation.Value);
            CustomAsserts.AreEqualVector(new Vector3(16, 17, 18), sourceTransform.Scale.Value);
        }

        [Test]
        public void GlobalPositionNoParent()
        {
            var testScene = new DclScene();

            var testEntity = new DclEntity(Guid.NewGuid(), "testEntity");
            testScene.AddEntity(testEntity);

            var testTransform = new DclTransformComponent(new Vector3(1, 2, 3), Quaternion.Euler(4, 5, 6), new Vector3(7, 8, 9));
            testEntity.AddComponent(testTransform);

            CustomAsserts.AreEqualVector(new Vector3(1, 2, 3), testTransform.GlobalPosition);
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

            CustomAsserts.AreEqualVector(new Vector3(11, 13, 15), testTransform.GlobalPosition);

            parentTransform.Scale.SetFloatingValue(new Vector3(2, 2, 2));

            CustomAsserts.AreEqualVector(new Vector3(21, 24, 27), testTransform.GlobalPosition);

            parentTransform.Rotation.SetFloatingValue(Quaternion.Euler(0, -90, 0));

            CustomAsserts.AreEqualVector(new Vector3(-23, 24, 23), testTransform.GlobalPosition);
        }

        [Test]
        public void SetPositionNoParent()
        {
            var testScene = new DclScene();

            var testEntity = new DclEntity(Guid.NewGuid(), "testEntity");
            testScene.AddEntity(testEntity);

            var testTransform = new DclTransformComponent(new Vector3(1, 2, 3), Quaternion.Euler(4, 5, 6), new Vector3(7, 8, 9));
            testEntity.AddComponent(testTransform);

            // set local position
            testTransform.Position.SetFloatingValue(new Vector3(10, 11, 12));

            CustomAsserts.AreEqualVector(new Vector3(10, 11, 12), testTransform.Position.Value);

            // set global position
            testTransform.GlobalPosition = new Vector3(20, 21, 22);

            CustomAsserts.AreEqualVector(new Vector3(20, 21, 22), testTransform.GlobalPosition);
        }

        [Test]
        public void SetPositionWithParent()
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

            // set local position
            testTransform.Position.SetFloatingValue(new Vector3(20, 21, 22));

            CustomAsserts.AreEqualVector(new Vector3(20, 21, 22), testTransform.Position.Value); // Check local
            CustomAsserts.AreEqualVector(new Vector3(21, 23, 25), testTransform.GlobalPosition); // Check global

            // set global position
            testTransform.GlobalPosition = new Vector3(30, 31, 32);

            CustomAsserts.AreEqualVector(new Vector3(29, 29, 29), testTransform.Position.Value); // Check local
            CustomAsserts.AreEqualVector(new Vector3(30, 31, 32), testTransform.GlobalPosition); // Check global
        }

        [Test]
        public void SetGlobalPositionWithVariousParents()
        {
            var testScene = new DclScene();

            var parentEntity = new DclEntity(Guid.NewGuid(), "testEntity");
            testScene.AddEntity(parentEntity);

            var parentTransform = new DclTransformComponent(new Vector3(8, 0, 8), Quaternion.Euler(0, 90, 0), Vector3.one);
            parentEntity.AddComponent(parentTransform);

            var testEntity = new DclEntity(Guid.NewGuid(), "testEntity", parentEntity.Id);
            testScene.AddEntity(testEntity);

            var testTransform = new DclTransformComponent(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one);
            testEntity.AddComponent(testTransform);

            testTransform.GlobalPosition = new Vector3(9, 0, 8);

            CustomAsserts.AreEqualVector(new Vector3(0, 0, 1), testTransform.Position.Value); // Check local
            CustomAsserts.AreEqualVector(new Vector3(9, 0, 8), testTransform.GlobalPosition); // Check global
        }
    }
}
