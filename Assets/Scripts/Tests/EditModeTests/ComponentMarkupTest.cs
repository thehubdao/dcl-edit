using System;
using System.Collections;
using System.IO;
using Assets.Scripts.System;
using Assets.Scripts.Tests.EditModeTests.TestUtility;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class ComponentMarkupTest
    {
        [Test]
        public void FilterComments()
        {
            var nl = "\r\n";
            var componentMarkupSystem = new CustomComponentMarkupSystem();

            {
                var result = componentMarkupSystem.FilterComments(@"Some code
/*First comment first line
First comment second line
*/
Some other code
");

                Assert.AreEqual($"First comment first line{nl}First comment second line{nl}", result);
            }
            {
                var result = componentMarkupSystem.FilterComments(@"Some code
// Line comment
Some other code
");

                Assert.AreEqual($" Line comment{nl}", result);
            }
            {
                var result = componentMarkupSystem.FilterComments(@"Some code
/*First comment first line
First comment second line
*/
// Line comment
Some other code
");

                Assert.AreEqual($"First comment first line{nl}First comment second line{nl} Line comment{nl}", result);
            }
            {
                var result = componentMarkupSystem.FilterComments(@"Some code
/*First comment first line
// Line comment in block comment
First comment second line
*/
Some other code
");

                Assert.AreEqual($"First comment first line{nl}// Line comment in block comment{nl}First comment second line{nl}", result);
            }
            {
                var result = componentMarkupSystem.FilterComments(@"Some code
                other code // Line comment
    // second line comment
                Some other code
                ");

                Assert.AreEqual($" Line comment{nl} second line comment{nl}", result);
            }
        }

        [Test]
        public void FindCustomComponentMarkups()
        {
            var componentMarkupSystem = new CustomComponentMarkupSystem();
            var pathState = new MockPathState("custom-component-markup");

            componentMarkupSystem.FindCustomComponentMarkups(Path.Combine(pathState.ProjectPath, "come_code.ts"));
        }
    }
}
