using System.Collections;
using Assets.Scripts.EditorState;
using Assets.Scripts.Utility;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Assets.Scripts.Tests.PlayModeTests.SystemTests
{
    public class CameraSystemTest
    {
        private IEnumerator SetupScene()
        {
            SceneManager.LoadScene(0);
            yield return null;
        }

        [UnityTest]
        public IEnumerator CameraStartupTest()
        {
            yield return SetupScene();
            Assert.Greater(EditorStates.CurrentCameraState.Position.y, 1);
            Assert.AreEqual(45, EditorStates.CurrentCameraState.Yaw);
            Assert.AreEqual(45, EditorStates.CurrentCameraState.Pitch);
        }

        // Test the movement of the Camera
        [UnityTest]
        public IEnumerator CameraMovementTest()
        {
            yield return SetupScene();

            float mouseSensitivity = PersistentData.MouseSensitivity;
            float cameraSpeed = PersistentData.CameraSpeed;

            EditorStates.CurrentCameraState.Position = new Vector3(0, 0, 0);
            EditorStates.CurrentCameraState.Yaw = 0;
            EditorStates.CurrentCameraState.Pitch = 0;

            EditorStates.CurrentCameraState.MoveFixed(new Vector3(1, 2, 3));
            Assert.AreEqual(new Vector3(1, 2, 3), EditorStates.CurrentCameraState.Position);
            Assert.AreEqual(0, EditorStates.CurrentCameraState.Yaw);
            Assert.AreEqual(0, EditorStates.CurrentCameraState.Pitch);

            EditorStates.CurrentCameraState.MoveFixed(new Vector3(3, 2, 1));
            Assert.AreEqual(new Vector3(4, 4, 4), EditorStates.CurrentCameraState.Position);
            Assert.AreEqual(0, EditorStates.CurrentCameraState.Yaw);
            Assert.AreEqual(0, EditorStates.CurrentCameraState.Pitch);

            EditorStates.CurrentCameraState.MoveStep(new Vector3(-2, 0, 0) / cameraSpeed, false);
            Assert.AreEqual(new Vector3(2, 4, 4), EditorStates.CurrentCameraState.Position);

            EditorStates.CurrentCameraState.Yaw = 90;
            EditorStates.CurrentCameraState.Pitch = 36;


            EditorStates.CurrentCameraState.MoveStep(new Vector3(2, 0, 0) / cameraSpeed / 3, true);
            Assert.IsTrue(Vector3.Distance(new Vector3(2, 4, 2), EditorStates.CurrentCameraState.Position) < 0.001,
                "The position was expected to be (2,4,2) but was " + EditorStates.CurrentCameraState.Position);


            EditorStates.CurrentCameraState.Position = new Vector3(0, 0, 0);
            EditorStates.CurrentCameraState.Yaw = 90;
            EditorStates.CurrentCameraState.Pitch = 0;

            EditorStates.CurrentCameraState.MoveContinuously(new Vector3(0, 0, 1), false);
            Assert.IsTrue(Vector3.Distance((new Vector3(1, 0, 0) * cameraSpeed) * Time.deltaTime, EditorStates.CurrentCameraState.Position) < 0.001,
                "The position was expected to be " + (new Vector3(1, 0, 0) * cameraSpeed) * Time.deltaTime + " but was " + EditorStates.CurrentCameraState.Position);


            EditorStates.CurrentCameraState.Position = new Vector3(0, 0, 0);
            EditorStates.CurrentCameraState.Yaw = 90;
            EditorStates.CurrentCameraState.Pitch = 0;

            EditorStates.CurrentCameraState.MoveContinuously(new Vector3(0, 0, 1), true);
            Assert.IsTrue(Vector3.Distance((new Vector3(1, 0, 0) * cameraSpeed * 3) * Time.deltaTime, EditorStates.CurrentCameraState.Position) < 0.001,
                "The position was expected to be " + (new Vector3(1, 0, 0) * cameraSpeed * 3) * Time.deltaTime + " but was " + EditorStates.CurrentCameraState.Position);


        }

        // Test the movement of the Camera
        [UnityTest]
        public IEnumerator CameraRotationTest()
        {
            yield return SetupScene();

            float mouseSensitivity = PersistentData.MouseSensitivity;

            EditorStates.CurrentCameraState.Position = new Vector3(0, 0, 0);
            EditorStates.CurrentCameraState.Yaw = 0;
            EditorStates.CurrentCameraState.Pitch = 0;

            EditorStates.CurrentCameraState.MoveFixed(new Vector3(1, 2, 3));
            Assert.AreEqual(new Vector3(1, 2, 3), EditorStates.CurrentCameraState.Position);
            Assert.AreEqual(0, EditorStates.CurrentCameraState.Yaw);
            Assert.AreEqual(0, EditorStates.CurrentCameraState.Pitch);

            EditorStates.CurrentCameraState.RotateStep(90 / mouseSensitivity, 0);
            Assert.AreEqual(new Vector3(1, 2, 3), EditorStates.CurrentCameraState.Position);
            Assert.AreEqual(90, EditorStates.CurrentCameraState.Yaw);
            Assert.AreEqual(0, EditorStates.CurrentCameraState.Pitch);

            EditorStates.CurrentCameraState.RotateStep(new Vector2(0, 90 / mouseSensitivity));
            Assert.AreEqual(new Vector3(1, 2, 3), EditorStates.CurrentCameraState.Position);
            Assert.AreEqual(90, EditorStates.CurrentCameraState.Yaw);
            Assert.AreEqual(-90, EditorStates.CurrentCameraState.Pitch);

            EditorStates.CurrentCameraState.RotateStep(-90 / mouseSensitivity, -90 / mouseSensitivity);
            Assert.AreEqual(new Vector3(1, 2, 3), EditorStates.CurrentCameraState.Position);
            Assert.AreEqual(0, EditorStates.CurrentCameraState.Yaw);
            Assert.AreEqual(0, EditorStates.CurrentCameraState.Pitch);

            EditorStates.CurrentCameraState.RotateAroundPointStep(new Vector3(2, 2, 3), new Vector2(-90 / mouseSensitivity, 0));
            Assert.AreEqual(new Vector3(2, 2, 2), EditorStates.CurrentCameraState.Position);
            Assert.AreEqual(-90, EditorStates.CurrentCameraState.Yaw);
            Assert.AreEqual(0, EditorStates.CurrentCameraState.Pitch);

        }

        // Test direction vectors
        [UnityTest]
        public IEnumerator DirectionVectorsTest()
        {
            yield return SetupScene();

            float mouseSensitivity = PersistentData.MouseSensitivity;

            EditorStates.CurrentCameraState.Position = new Vector3(1, 2, 3);
            EditorStates.CurrentCameraState.Yaw = 0;
            EditorStates.CurrentCameraState.Pitch = 0;

            Assert.AreEqual(new Vector3(1, 2, 3), EditorStates.CurrentCameraState.Position);
            Assert.AreEqual(0, EditorStates.CurrentCameraState.Yaw);
            Assert.AreEqual(0, EditorStates.CurrentCameraState.Pitch);

            Assert.AreEqual(new Vector3(0, 0, 1), EditorStates.CurrentCameraState.Forward);
            Assert.AreEqual(new Vector3(0, 0, -1), EditorStates.CurrentCameraState.Back);
            Assert.AreEqual(new Vector3(1, 0, 0), EditorStates.CurrentCameraState.Right);
            Assert.AreEqual(new Vector3(-1, 0, 0), EditorStates.CurrentCameraState.Left);
            Assert.AreEqual(new Vector3(0, 1, 0), EditorStates.CurrentCameraState.Up);
            Assert.AreEqual(new Vector3(0, -1, 0), EditorStates.CurrentCameraState.Down);

            EditorStates.CurrentCameraState.RotateStep(90 / mouseSensitivity, 0);

            Assert.IsTrue(Vector3.Distance(new Vector3(1, 0, 0), EditorStates.CurrentCameraState.Forward) < 0.001);
            Assert.IsTrue(Vector3.Distance(new Vector3(-1, 0, 0), EditorStates.CurrentCameraState.Back) < 0.001);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, 0, -1), EditorStates.CurrentCameraState.Right) < 0.001);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, 0, 1), EditorStates.CurrentCameraState.Left) < 0.001);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, 1, 0), EditorStates.CurrentCameraState.Up) < 0.001);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, -1, 0), EditorStates.CurrentCameraState.Down) < 0.001);

            EditorStates.CurrentCameraState.RotateStep(new Vector2(0, 90 / mouseSensitivity));

            Assert.IsTrue(Vector3.Distance(new Vector3(0, 1, 0), EditorStates.CurrentCameraState.Forward) < 0.001, "The Forward vector was expected to be (0,1,0) but was " + EditorStates.CurrentCameraState.Forward);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, -1, 0), EditorStates.CurrentCameraState.Back) < 0.001, "The Back vector was expected to be (0,-1,0) but was " + EditorStates.CurrentCameraState.Back);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, 0, -1), EditorStates.CurrentCameraState.Right) < 0.001, "The Right vector was expected to be (0,0,-1) but was " + EditorStates.CurrentCameraState.Right);
            Assert.IsTrue(Vector3.Distance(new Vector3(0, 0, 1), EditorStates.CurrentCameraState.Left) < 0.001, "The Left vector was expected to be (0,0,1) but was " + EditorStates.CurrentCameraState.Left);
            Assert.IsTrue(Vector3.Distance(new Vector3(-1, 0, 0), EditorStates.CurrentCameraState.Up) < 0.001, "The Up vector was expected to be (-1,0,0) but was " + EditorStates.CurrentCameraState.Up);
            Assert.IsTrue(Vector3.Distance(new Vector3(1, 0, 0), EditorStates.CurrentCameraState.Down) < 0.001, "The Down vector was expected to be (1,0,0) but was " + EditorStates.CurrentCameraState.Down);
        }

        // Test the Pitch limits
        [UnityTest]
        public IEnumerator PitchLimitsTest()
        {
            yield return SetupScene();

            float mouseSensitivity = PersistentData.MouseSensitivity;

            EditorStates.CurrentCameraState.Position = new Vector3(0, 0, 0);
            EditorStates.CurrentCameraState.Yaw = 0;
            EditorStates.CurrentCameraState.Pitch = 0;

            EditorStates.CurrentCameraState.Pitch = 90;
            Assert.AreEqual(90, EditorStates.CurrentCameraState.Pitch);

            EditorStates.CurrentCameraState.Pitch = -90;
            Assert.AreEqual(-90, EditorStates.CurrentCameraState.Pitch);

            EditorStates.CurrentCameraState.Pitch = 180;
            Assert.AreEqual(100, EditorStates.CurrentCameraState.Pitch);

            EditorStates.CurrentCameraState.Pitch = -180;
            Assert.AreEqual(-100, EditorStates.CurrentCameraState.Pitch);

            EditorStates.CurrentCameraState.RotateStep(30 / mouseSensitivity, -100 / mouseSensitivity);
            Assert.AreEqual(0, EditorStates.CurrentCameraState.Pitch, 0.001);

            EditorStates.CurrentCameraState.RotateStep(30 / mouseSensitivity, -150 / mouseSensitivity);
            Assert.AreEqual(100, EditorStates.CurrentCameraState.Pitch, 0.001);

            EditorStates.CurrentCameraState.RotateStep(30 / mouseSensitivity, 300 / mouseSensitivity);
            Assert.AreEqual(-100, EditorStates.CurrentCameraState.Pitch, 0.001);
        }
    }
}
