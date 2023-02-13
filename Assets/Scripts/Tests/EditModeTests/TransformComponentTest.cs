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
            CustomAsserts.AreEqualVector(new Vector3(1, 2, 3), testTransform.position.Value);
            Assert.AreEqual(Quaternion.Euler(4, 5, 6), testTransform.rotation.Value);
            CustomAsserts.AreEqualVector(new Vector3(7, 8, 9), testTransform.scale.Value);

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
            testTransform.position.SetFloatingValue(new Vector3(10, 11, 12));
            testTransform.rotation.SetFloatingValue(Quaternion.Euler(13, 14, 15));
            testTransform.scale.SetFloatingValue(new Vector3(16, 17, 18));

            // get properties from build in shortcuts
            CustomAsserts.AreEqualVector(new Vector3(10, 11, 12), testTransform.position.Value);
            Assert.AreEqual(Quaternion.Euler(13, 14, 15), testTransform.rotation.Value);
            CustomAsserts.AreEqualVector(new Vector3(16, 17, 18), testTransform.scale.Value);

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
            CustomAsserts.AreEqualVector(new Vector3(1, 2, 3), testTransform.position.Value);
            Assert.AreEqual(Quaternion.Euler(4, 5, 6), testTransform.rotation.Value);
            CustomAsserts.AreEqualVector(new Vector3(7, 8, 9), testTransform.scale.Value);

            // set properties
            testTransform.position.SetFloatingValue(new Vector3(10, 11, 12));
            testTransform.rotation.SetFloatingValue(Quaternion.Euler(13, 14, 15));
            testTransform.scale.SetFloatingValue(new Vector3(16, 17, 18));

            // get properties from source transform
            CustomAsserts.AreEqualVector(new Vector3(10, 11, 12), sourceTransform.position.Value);
            Assert.AreEqual(Quaternion.Euler(13, 14, 15), sourceTransform.rotation.Value);
            CustomAsserts.AreEqualVector(new Vector3(16, 17, 18), sourceTransform.scale.Value);
        }

        [Test]
        public void GlobalPositionNoParent()
        {
            var testScene = new DclScene();

            var testEntity = new DclEntity(Guid.NewGuid(), "testEntity");
            testScene.AddEntity(testEntity);

            var testTransform = new DclTransformComponent(new Vector3(1, 2, 3), Quaternion.Euler(4, 5, 6), new Vector3(7, 8, 9));
            testEntity.AddComponent(testTransform);

            CustomAsserts.AreEqualVector(new Vector3(1, 2, 3), testTransform.globalPosition);
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

            CustomAsserts.AreEqualVector(new Vector3(11, 13, 15), testTransform.globalPosition);

            parentTransform.scale.SetFloatingValue(new Vector3(2, 2, 2));

            CustomAsserts.AreEqualVector(new Vector3(21, 24, 27), testTransform.globalPosition);

            parentTransform.rotation.SetFloatingValue(Quaternion.Euler(0, -90, 0));

            CustomAsserts.AreEqualVector(new Vector3(-23, 24, 23), testTransform.globalPosition);
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
            testTransform.position.SetFloatingValue(new Vector3(10, 11, 12));

            CustomAsserts.AreEqualVector(new Vector3(10, 11, 12), testTransform.position.Value);

            // set global position
            testTransform.globalPosition = new Vector3(20, 21, 22);

            CustomAsserts.AreEqualVector(new Vector3(20, 21, 22), testTransform.globalPosition);
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
            testTransform.position.SetFloatingValue(new Vector3(20, 21, 22));

            CustomAsserts.AreEqualVector(new Vector3(20, 21, 22), testTransform.position.Value); // Check local
            CustomAsserts.AreEqualVector(new Vector3(21, 23, 25), testTransform.globalPosition); // Check global

            // set global position
            testTransform.globalPosition = new Vector3(30, 31, 32);

            CustomAsserts.AreEqualVector(new Vector3(29, 29, 29), testTransform.position.Value); // Check local
            CustomAsserts.AreEqualVector(new Vector3(30, 31, 32), testTransform.globalPosition); // Check global
        }

        [Test]
        public void SetGlobalPositionWithRotatedParents()
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

            testTransform.globalPosition = new Vector3(9, 0, 8);

            CustomAsserts.AreEqualVector(new Vector3(0, 0, 1), testTransform.position.Value); // Check local
            CustomAsserts.AreEqualVector(new Vector3(9, 0, 8), testTransform.globalPosition); // Check global
        }

        [Test]
        public void SetGlobalPositionWithScaledParents()
        {
            var testScene = new DclScene();

            var parentEntity = new DclEntity(Guid.NewGuid(), "testEntity");
            testScene.AddEntity(parentEntity);

            var parentTransform = new DclTransformComponent(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(2, 1, 0.5f));
            parentEntity.AddComponent(parentTransform);

            var testEntity = new DclEntity(Guid.NewGuid(), "testEntity", parentEntity.Id);
            testScene.AddEntity(testEntity);

            var testTransform = new DclTransformComponent(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one);
            testEntity.AddComponent(testTransform);

            testTransform.globalPosition = new Vector3(2, 0, 4);

            CustomAsserts.AreEqualVector(new Vector3(1, 0, 8), testTransform.position.Value); // Check local
            CustomAsserts.AreEqualVector(new Vector3(2, 0, 4), testTransform.globalPosition); // Check global
        }

        [Test]
        public void SetGlobalPositionWithScaledAndRotatedParents()
        {
            var testScene = new DclScene();

            var parentEntity = new DclEntity(Guid.NewGuid(), "testEntity");
            testScene.AddEntity(parentEntity);

            var parentTransform = new DclTransformComponent(new Vector3(0, 0, 0), Quaternion.Euler(0, 90, 0), new Vector3(2, 1, 0.5f));
            parentEntity.AddComponent(parentTransform);

            var testEntity = new DclEntity(Guid.NewGuid(), "testEntity", parentEntity.Id);
            testScene.AddEntity(testEntity);

            var testTransform = new DclTransformComponent(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one);
            testEntity.AddComponent(testTransform);

            testTransform.globalPosition = new Vector3(2, 0, 4);

            CustomAsserts.AreEqualVector(new Vector3(-2, 0, 4), testTransform.position.Value); // Check local
            CustomAsserts.AreEqualVector(new Vector3(2, 0, 4), testTransform.globalPosition); // Check global

            // add a third entity to test the global position of the child
            var testEntity2 = new DclEntity(Guid.NewGuid(), "testEntity2", testEntity.Id);
            testScene.AddEntity(testEntity2);

            var testTransform2 = new DclTransformComponent(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one);
            testEntity2.AddComponent(testTransform2);

            testTransform2.globalPosition = new Vector3(4, 0, 6);

            CustomAsserts.AreEqualVector(new Vector3(-1, 0, 4), testTransform2.position.Value); // Check local
            CustomAsserts.AreEqualVector(new Vector3(4, 0, 6), testTransform2.globalPosition); // Check global
        }
    }
}
