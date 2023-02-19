using NUnit.Framework;
using UnityEngine;

namespace Assets.Scripts.Tests.EditModeTests.TestUtility
{
    public static class CustomAsserts
    {
        private const float delta = 0.000002f;

        public static void AreEqualVector(Vector3 expected, Vector3 actual)
        {
            Assert.AreEqual(expected.x, actual.x, delta);
            Assert.AreEqual(expected.y, actual.y, delta);
            Assert.AreEqual(expected.z, actual.z, delta);
        }

        public static void AreEqualVector(Vector2 expected, Vector2 actual)
        {
            Assert.AreEqual(expected.x, actual.x, delta);
            Assert.AreEqual(expected.y, actual.y, delta);
        }

        public static void AreEqualQuaternion(Quaternion expected, Quaternion actual)
        {
            Assert.IsTrue(Quaternion.Angle(expected, actual) < delta,
                $"Expected: {expected.eulerAngles}\nBut was: {actual.eulerAngles} (Error of {Quaternion.Angle(expected, actual)} degrees)");
        }
    }
}
