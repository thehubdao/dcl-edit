using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.Utility;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Assets.Scripts.Tests.PlayModeTests.SystemTests
{
    public class CameraSystemTest
    {
        // Dependencies
        private CameraState _cameraState;
        private EditorEvents _editorEvents;

        public CameraSystemTest(EditorEvents editorEvents)
        {
            _editorEvents = editorEvents;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _cameraState = new CameraState(_editorEvents);
        }

        private IEnumerator SetupScene()
        {
            SceneManager.LoadScene(0);
            yield return null;
        }

        [UnityTest]
        public IEnumerator CameraStartupTest()
        {
            yield return SetupScene();
            Assert.Greater(_cameraState.Position.y, 1);
            Assert.AreEqual(45, _cameraState.Yaw);
            Assert.AreEqual(45, _cameraState.Pitch);
        }

        // Test the movement of the Camera
        [UnityTest]
        public IEnumerator CameraMovementTest()
        {
            yield return SetupScene();

            float mouseSensitivity = PersistentData.MouseSensitivity;
            float cameraSpeed = PersistentData.CameraSpeed;

            _cameraState.Position = new Vector3(0, 0, 0);
            _cameraState.Yaw = 0;
            _cameraState.Pitch = 0;

            _cameraState.MoveFixed(new Vector3(1, 2, 3));
            Assert.AreEqual(new Vector3(1, 2, 3), _cameraState.Position);
            Assert.AreEqual(0, _cameraState.Yaw);
            Assert.AreEqual(0, _cameraState.Pitch);

            _cameraState.MoveFixed(new Vector3(3, 2, 1));
            Assert.AreEqual(new Vector3(4, 4, 4), _cameraState.Position);
            Assert.AreEqual(0, _cameraState.Yaw);
            Assert.AreEqual(0, _cameraState.Pitch);

            _cameraState.MoveStep(new Vector3(-2, 0, 0) / cameraSpeed, false);
            Assert.AreEqual(new Vector3(2, 4, 4), _cameraState.Position);

            _cameraState.Yaw = 90;
            _cameraState.Pitch = 36;


            _cameraState.MoveStep(new Vector3(2, 0, 0) / cameraSpeed / 3, true);
            Assert.IsTrue(Vector3.Distance(new Vector3(2, 4, 2), _cameraState.Position) < 0.001,
                "The position was expected to be (2,4,2) but was " + _cameraState.Position);


            _cameraState.Position = new Vector3(0, 0, 0);
            _cameraState.Yaw = 90;
            _cameraState.Pitch = 0;

            _cameraState.MoveContinuously(new Vector3(0, 0, 1), false);
            Assert.IsTrue(Vector3.Distance((new Vector3(1, 0, 0) * cameraSpeed) * Time.deltaTime, _cameraState.Position) < 0.001,
                "The position was expected to be " + (new Vector3(1, 0, 0) * cameraSpeed) * Time.deltaTime + " but was " + _cameraState.Position);


            _cameraState.Position = new Vector3(0, 0, 0);
            _cameraState.Yaw = 90;
            _cameraState.Pitch = 0;

            _cameraState.MoveContinuously(new Vector3(0, 0, 1), true);
            Assert.IsTrue(Vector3.Distance((new Vector3(1, 0, 0) * cameraSpeed * 3) * Time.deltaTime, _cameraState.Position) < 0.001,
                "The position was expected to be " + (new Vector3(1, 0, 0) * cameraSpeed * 3) * Time.deltaTime + " but was " + _cameraState.Position);
        }

        // Test the movement of the Camera
        [UnityTest]
        public IEnumerator CameraRotationTest()
        {
            yield return SetupScene();

            float mouseSensitivity = PersistentData.MouseSensitivity;

            _cameraState.Position = new Vector3(0, 0, 0);
            _cameraState.Yaw = 0;
            _cameraState.Pitch = 0;

            _cameraState.MoveFixed(new Vector3(1, 2, 3));
            Assert.AreEqual(new Vector3(1, 2, 3), _cameraState.Position);
            Assert.AreEqual(0, _cameraState.Yaw);
            Assert.AreEqual(0, _cameraState.Pitch);

            _cameraState.RotateStep(90 / mouseSensitivity, 0);
            Assert.AreEqual(new Vector3(1, 2, 3), _cameraState.Position);
            Assert.AreEqual(90, _cameraState.Yaw);
            Assert.AreEqual(0, _cameraState.Pitch);

            _cameraState.RotateStep(new Vector2(0, 90 / mouseSensitivity));
            Assert.AreEqual(new Vector3(1, 2, 3), _cameraState.Position);
            Assert.AreEqual(90, _cameraState.Yaw);
            Assert.AreEqual(-90, _cameraState.Pitch);

            _cameraState.RotateStep(-90 / mouseSensitivity, -90 / mouseSensitivity);
            Assert.AreEqual(new Vector3(1, 2, 3), _cameraState.Position);
            Assert.AreEqual(0, _cameraState.Yaw);
            Assert.AreEqual(0, _cameraState.Pitch);

            _cameraState.RotateAroundPointStep(new Vector3(2, 2, 3), new Vector2(-90 / mouseSensitivity, 0));
            Assert.AreEqual(new Vector3(2, 2, 2), _cameraState.Position);
            Assert.AreEqual(-90, _cameraState.Yaw);
            Assert.AreEqual(0, _cameraState.Pitch);
        }

        // Test direction vectors
        [UnityTest]
        public IEnumerator DirectionVectorsTest()
        {
            yield return SetupScene();

            float mouseSensitivity = PersistentData.MouseSensitivity;

            _cameraState.Position = new Vector3(1, 2, 3);
            _cameraState.Yaw = 0;
            _cameraState.Pitch = 0;

            Assert.AreEqual(new Vector3(1, 2, 3), _cameraState.Position);
            Assert.AreEqual(0, _cameraState.Yaw);
            Assert.AreEqual(0, _cameraState.Pitch);

            Assert.AreEqual(new Vector3(0, 0, 1), _cameraState.Forward);
            Assert.AreEqual(new Vector3(0, 0, -1), _cameraState.Back);
            Assert.AreEqual(new Vector3(1, 0, 0), _cameraState.Right);
            Assert.AreEqual(new Vector3(-1, 0, 0), _cameraState.Left);
            Assert.AreEqual(new Vector3(0, 1, 0), _cameraState.Up);
            Assert.AreEqual(new Vector3(0, -1, 0), _cameraState.Down);

            _cameraState.RotateStep(90 / mouseSensitivity, 0);

            Assert.IsTrue(Vector3.Distance(new Vector3(1, 0, 0), _cameraState.Forward) < 0.001);
            Assert.IsTrue(Vector3.Distance(new Vector3(-1, 0, 0), _cameraState.Back) < 0.001);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, 0, -1), _cameraState.Right) < 0.001);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, 0, 1), _cameraState.Left) < 0.001);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, 1, 0), _cameraState.Up) < 0.001);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, -1, 0), _cameraState.Down) < 0.001);

            _cameraState.RotateStep(new Vector2(0, 90 / mouseSensitivity));

            Assert.IsTrue(Vector3.Distance(new Vector3(0, 1, 0), _cameraState.Forward) < 0.001, "The Forward vector was expected to be (0,1,0) but was " + _cameraState.Forward);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, -1, 0), _cameraState.Back) < 0.001, "The Back vector was expected to be (0,-1,0) but was " + _cameraState.Back);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, 0, -1), _cameraState.Right) < 0.001, "The Right vector was expected to be (0,0,-1) but was " + _cameraState.Right);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, 0, 1), _cameraState.Left) < 0.001, "The Left vector was expected to be (0,0,1) but was " + _cameraState.Left);
            Assert.IsTrue(Vector3.Distance(new Vector3(-1, 0, 0), _cameraState.Up) < 0.001, "The Up vector was expected to be (-1,0,0) but was " + _cameraState.Up);
            Assert.IsTrue(Vector3.Distance(new Vector3(1, 0, 0), _cameraState.Down) < 0.001, "The Down vector was expected to be (1,0,0) but was " + _cameraState.Down);
        }

        // Test the Pitch limits
        [UnityTest]
        public IEnumerator PitchLimitsTest()
        {
            yield return SetupScene();

            float mouseSensitivity = PersistentData.MouseSensitivity;

            _cameraState.Position = new Vector3(0, 0, 0);
            _cameraState.Yaw = 0;
            _cameraState.Pitch = 0;

            _cameraState.Pitch = 90;
            Assert.AreEqual(90, _cameraState.Pitch);

            _cameraState.Pitch = -90;
            Assert.AreEqual(-90, _cameraState.Pitch);

            _cameraState.Pitch = 180;
            Assert.AreEqual(100, _cameraState.Pitch);

            _cameraState.Pitch = -180;
            Assert.AreEqual(-100, _cameraState.Pitch);

            _cameraState.RotateStep(30 / mouseSensitivity, -100 / mouseSensitivity);
            Assert.AreEqual(0, _cameraState.Pitch, 0.001);

            _cameraState.RotateStep(30 / mouseSensitivity, -150 / mouseSensitivity);
            Assert.AreEqual(100, _cameraState.Pitch, 0.001);

            _cameraState.RotateStep(30 / mouseSensitivity, 300 / mouseSensitivity);
            Assert.AreEqual(-100, _cameraState.Pitch, 0.001);
        }
    }
}