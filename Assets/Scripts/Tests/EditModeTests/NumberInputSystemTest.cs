using Assets.Scripts.System;
using NUnit.Framework;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class NumberInputSystemTest
    {
        private float roundingError = 0.0001f;
            
        [Test]
        public void ValidateNumberInputTest()
        {
            var numberInputSystem = new NumberInputSystem();
            Assert.AreEqual(1f, numberInputSystem.ValidateNumberInput("1"), roundingError);
            Assert.AreEqual(1f, numberInputSystem.ValidateNumberInput("1.0"), roundingError);
            Assert.AreEqual(-1f, numberInputSystem.ValidateNumberInput("-1"));
            Assert.AreEqual(-1f, numberInputSystem.ValidateNumberInput("-1"), roundingError);
            Assert.AreEqual(-1f, numberInputSystem.ValidateNumberInput("-1.0"), roundingError);
            Assert.AreEqual(1.1f, numberInputSystem.ValidateNumberInput("1.1"), roundingError);
            Assert.AreEqual(1.1f, numberInputSystem.ValidateNumberInput("1,1"));
            Assert.AreEqual(100000000f, numberInputSystem.ValidateNumberInput("100000000"), roundingError);
            Assert.AreEqual(0.0000001f, numberInputSystem.ValidateNumberInput("0.0000001"), roundingError);
            Assert.AreEqual(null, numberInputSystem.ValidateNumberInput("a"));
            Assert.AreEqual(null, numberInputSystem.ValidateNumberInput("A"));
            Assert.AreEqual(1f, numberInputSystem.ValidateNumberInput(" 1"));
            Assert.AreEqual(1f, numberInputSystem.ValidateNumberInput("1 "));
            Assert.AreEqual(null, numberInputSystem.ValidateNumberInput(" 1 2 "));
            Assert.AreEqual(null, numberInputSystem.ValidateNumberInput(""));
            Assert.AreEqual(null, numberInputSystem.ValidateNumberInput("1a"));
            Assert.AreEqual(null, numberInputSystem.ValidateNumberInput("a1"));
            Assert.AreEqual(null, numberInputSystem.ValidateNumberInput("$"));
            Assert.AreEqual(null,numberInputSystem.ValidateNumberInput("%"));
            Assert.AreEqual(null,numberInputSystem.ValidateNumberInput("/"));
            Assert.AreEqual(null,numberInputSystem.ValidateNumberInput(":"));
            Assert.AreEqual(null,numberInputSystem.ValidateNumberInput("."));
            Assert.AreEqual(null,numberInputSystem.ValidateNumberInput(","));
            Assert.AreEqual(null,numberInputSystem.ValidateNumberInput("-"));
            Assert.AreEqual(null,numberInputSystem.ValidateNumberInput(" "));
            Assert.AreEqual(null,numberInputSystem.ValidateNumberInput("\n"));
            Assert.AreEqual(null,numberInputSystem.ValidateNumberInput("\t"));
            Assert.AreEqual(null,numberInputSystem.ValidateNumberInput("<"));
            Assert.AreEqual(null, numberInputSystem.ValidateNumberInput("1. 01"));
            Assert.AreEqual(null, numberInputSystem.ValidateNumberInput("1.\n01"));
            Assert.AreEqual(null, numberInputSystem.ValidateNumberInput("1.\t01"));
            Assert.AreEqual(1.01f, numberInputSystem.ValidateNumberInput("1..01"));
            Assert.AreEqual(1.01f, numberInputSystem.ValidateNumberInput("1.,01"));
            Assert.AreEqual(1.01f, numberInputSystem.ValidateNumberInput("1,.01"));
            Assert.AreEqual(null, numberInputSystem.ValidateNumberInput("1-"));
        }
    }
}