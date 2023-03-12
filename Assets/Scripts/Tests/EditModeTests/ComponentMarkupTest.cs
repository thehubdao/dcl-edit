using System;
using System.Collections;
using System.Collections.Generic;
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
            var componentMarkupSystem = new CustomComponentMarkupSystem();

            {
                var result = componentMarkupSystem.FilterComments(@"Some code
/*First comment first line
First comment second line
*/
Some other code
");

                Assert.AreEqual(@"         
  First comment first line
First comment second line
  
               
", result);
            }
            {
                var result = componentMarkupSystem.FilterComments(@"Some code
// Line comment
Some other code
");

                Assert.AreEqual(@"         
   Line comment
               
", result);
            }
            {
                var result = componentMarkupSystem.FilterComments(@"Some code
/*First comment first line
First comment second line
*/
// Line comment
Some other code
");

                Assert.AreEqual(@"         
  First comment first line
First comment second line
  
   Line comment
               
", result);
            }
            {
                var result = componentMarkupSystem.FilterComments(@"Some code
/*First comment first line
// Line comment in block comment
First comment second line
*/
Some other code
");

                Assert.AreEqual(@"         
  First comment first line
// Line comment in block comment
First comment second line
  
               
", result);
            }
            {
                var result = componentMarkupSystem.FilterComments(@"Some code
                other code // Line comment
    // second line comment
                Some other code
                ");

                Assert.AreEqual(@"         
                              Line comment
       second line comment
                               
                ", result);
            }
        }

        [Test]
        public void FindCustomComponentMarkups()
        {
            var componentMarkupSystem = new CustomComponentMarkupSystem();
            var pathState = new MockPathState("custom-component-markup");

            componentMarkupSystem.FindCustomComponentMarkups(Path.Combine(pathState.ProjectPath, "come_code.ts"), new List<IFileReadingProblem>());
        }
    }
}
