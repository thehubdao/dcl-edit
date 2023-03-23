using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using Assets.Scripts.Tests.EditModeTests.TestUtility;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using static Assets.Scripts.SceneState.DclComponent.DclComponentProperty;

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

        private (AvailableComponentsState, List<IFileReadingProblem>, CustomComponentMarkupSystem) SetupCustomComponentsTests()
        {
            var mockPath = new MockPathState("custom-components");

            var fileManagerSystem = new FileManagerSystem();
            fileManagerSystem.Construct(mockPath);

            var availableComponentsState = new AvailableComponentsState();

            var ccms = new CustomComponentMarkupSystem();
            ccms.Construct(fileManagerSystem, availableComponentsState, mockPath);

            var problems = ccms.SetupCustomComponents();

            return (availableComponentsState, problems, ccms);
        }

        [Test]
        public void CheckValidCustomComponents()
        {
            var (availableComponent, _, _) = SetupCustomComponentsTests();

            // Minimal
            var minimal = availableComponent.GetComponentDefinitionByName("Minimal");
            Assert.IsNotNull(minimal);

            Assert.AreEqual("Minimal", minimal.NameInCode);
            Assert.AreEqual("Minimal", minimal.NameOfSlot);

            Assert.AreEqual(0, minimal.properties.Count);
            Assert.AreEqual("src/valid_components", minimal.SourceFile);

            // Class and component
            var classAndComponent = availableComponent.GetComponentDefinitionByName("ClassAndComponent");
            Assert.IsNotNull(classAndComponent);

            Assert.AreEqual("ClassAndComponent", classAndComponent.NameInCode);
            Assert.AreEqual("ComponentName", classAndComponent.NameOfSlot);

            Assert.AreEqual(0, classAndComponent.properties.Count);
            Assert.AreEqual("src/valid_components", classAndComponent.SourceFile);

            // Class and number property
            var classAndNumberProperty = availableComponent.GetComponentDefinitionByName("ClassAndNumberProperty");
            Assert.IsNotNull(classAndNumberProperty);

            Assert.AreEqual("ClassAndNumberProperty", classAndNumberProperty.NameInCode);
            Assert.AreEqual("ClassAndNumberProperty", classAndNumberProperty.NameOfSlot);

            Assert.AreEqual(1, classAndNumberProperty.properties.Count);
            Assert.AreEqual("property1", classAndNumberProperty.properties[0].name);
            Assert.AreEqual(PropertyType.Float, classAndNumberProperty.properties[0].type);
            Assert.AreEqual(0f, classAndNumberProperty.properties[0].defaultValue);
            Assert.AreEqual(PropertyDefinition.Flags.None, classAndNumberProperty.properties[0].flags);

            Assert.AreEqual("src/valid_components", classAndNumberProperty.SourceFile);

            // Class and number property with default
            var classAndPropertyWithDefault = availableComponent.GetComponentDefinitionByName("ClassAndNumberPropertyWithDefault");
            Assert.IsNotNull(classAndPropertyWithDefault);

            Assert.AreEqual("ClassAndNumberPropertyWithDefault", classAndPropertyWithDefault.NameInCode);
            Assert.AreEqual("ClassAndNumberPropertyWithDefault", classAndPropertyWithDefault.NameOfSlot);

            Assert.AreEqual(1, classAndPropertyWithDefault.properties.Count);
            Assert.AreEqual("property1", classAndPropertyWithDefault.properties[0].name);
            Assert.AreEqual(PropertyType.Float, classAndPropertyWithDefault.properties[0].type);
            Assert.AreEqual(5f, classAndPropertyWithDefault.properties[0].defaultValue);
            Assert.AreEqual(PropertyDefinition.Flags.None, classAndPropertyWithDefault.properties[0].flags);

            Assert.AreEqual("src/valid_components", classAndPropertyWithDefault.SourceFile);

            // Class and string property
            var classAndStringProperty = availableComponent.GetComponentDefinitionByName("ClassAndStringProperty");
            Assert.IsNotNull(classAndStringProperty);

            Assert.AreEqual("ClassAndStringProperty", classAndStringProperty.NameInCode);
            Assert.AreEqual("ClassAndStringProperty", classAndStringProperty.NameOfSlot);

            Assert.AreEqual(1, classAndStringProperty.properties.Count);
            Assert.AreEqual("property1", classAndStringProperty.properties[0].name);
            Assert.AreEqual(PropertyType.String, classAndStringProperty.properties[0].type);
            Assert.AreEqual("", classAndStringProperty.properties[0].defaultValue);
            Assert.AreEqual(PropertyDefinition.Flags.None, classAndStringProperty.properties[0].flags);

            Assert.AreEqual("src/valid_components", classAndStringProperty.SourceFile);

            // Class and string property with default
            var classAndStringPropertyWithDefault = availableComponent.GetComponentDefinitionByName("ClassAndStringPropertyWithDefault");
            Assert.IsNotNull(classAndStringPropertyWithDefault);

            Assert.AreEqual("ClassAndStringPropertyWithDefault", classAndStringPropertyWithDefault.NameInCode);
            Assert.AreEqual("ClassAndStringPropertyWithDefault", classAndStringPropertyWithDefault.NameOfSlot);

            Assert.AreEqual(1, classAndStringPropertyWithDefault.properties.Count);
            Assert.AreEqual("property1", classAndStringPropertyWithDefault.properties[0].name);
            Assert.AreEqual(PropertyType.String, classAndStringPropertyWithDefault.properties[0].type);
            Assert.AreEqual("default", classAndStringPropertyWithDefault.properties[0].defaultValue);
            Assert.AreEqual(PropertyDefinition.Flags.None, classAndStringPropertyWithDefault.properties[0].flags);

            Assert.AreEqual("src/valid_components", classAndStringPropertyWithDefault.SourceFile);

            // Class and vector3 property
            var classAndVector3Property = availableComponent.GetComponentDefinitionByName("ClassAndVector3Property");
            Assert.IsNotNull(classAndVector3Property);

            Assert.AreEqual("ClassAndVector3Property", classAndVector3Property.NameInCode);
            Assert.AreEqual("ClassAndVector3Property", classAndVector3Property.NameOfSlot);

            Assert.AreEqual(1, classAndVector3Property.properties.Count);
            Assert.AreEqual("property1", classAndVector3Property.properties[0].name);
            Assert.AreEqual(PropertyType.Vector3, classAndVector3Property.properties[0].type);
            Assert.AreEqual(Vector3.zero, classAndVector3Property.properties[0].defaultValue);
            Assert.AreEqual(PropertyDefinition.Flags.None, classAndVector3Property.properties[0].flags);

            Assert.AreEqual("src/valid_components", classAndVector3Property.SourceFile);

            // Class and vector3 property with default
            var classAndVector3PropertyWithDefault = availableComponent.GetComponentDefinitionByName("ClassAndVector3PropertyWithDefault");
            Assert.IsNotNull(classAndVector3PropertyWithDefault);

            Assert.AreEqual("ClassAndVector3PropertyWithDefault", classAndVector3PropertyWithDefault.NameInCode);
            Assert.AreEqual("ClassAndVector3PropertyWithDefault", classAndVector3PropertyWithDefault.NameOfSlot);

            Assert.AreEqual(1, classAndVector3PropertyWithDefault.properties.Count);
            Assert.AreEqual("property1", classAndVector3PropertyWithDefault.properties[0].name);
            Assert.AreEqual(PropertyType.Vector3, classAndVector3PropertyWithDefault.properties[0].type);
            Assert.AreEqual(new Vector3(1, 2, 3), classAndVector3PropertyWithDefault.properties[0].defaultValue);
            Assert.AreEqual(PropertyDefinition.Flags.None, classAndVector3PropertyWithDefault.properties[0].flags);

            Assert.AreEqual("src/valid_components", classAndVector3PropertyWithDefault.SourceFile);

            // Class and multiple properties
            var classAndMultipleProperties = availableComponent.GetComponentDefinitionByName("ClassAndMultipleProperties");
            Assert.IsNotNull(classAndMultipleProperties);

            Assert.AreEqual("ClassAndMultipleProperties", classAndMultipleProperties.NameInCode);
            Assert.AreEqual("ClassAndMultipleProperties", classAndMultipleProperties.NameOfSlot);

            Assert.AreEqual(2, classAndMultipleProperties.properties.Count);

            Assert.AreEqual("property1", classAndMultipleProperties.properties[0].name);
            Assert.AreEqual(PropertyType.Float, classAndMultipleProperties.properties[0].type);
            Assert.AreEqual(0f, classAndMultipleProperties.properties[0].defaultValue);
            Assert.AreEqual(PropertyDefinition.Flags.None, classAndMultipleProperties.properties[0].flags);

            Assert.AreEqual("property2", classAndMultipleProperties.properties[1].name);
            Assert.AreEqual(PropertyType.String, classAndMultipleProperties.properties[1].type);
            Assert.AreEqual("", classAndMultipleProperties.properties[1].defaultValue);
            Assert.AreEqual(PropertyDefinition.Flags.None, classAndMultipleProperties.properties[1].flags);

            Assert.AreEqual("src/valid_components", classAndMultipleProperties.SourceFile);

            // Class with import file
            var classWithImportFile = availableComponent.GetComponentDefinitionByName("ClassWithImportFile");
            Assert.IsNotNull(classWithImportFile);

            Assert.AreEqual("ClassWithImportFile", classWithImportFile.NameInCode);
            Assert.AreEqual("ClassWithImportFile", classWithImportFile.NameOfSlot);

            Assert.AreEqual(0, classWithImportFile.properties.Count);
            Assert.AreEqual("src/components/class_with_import_file", classWithImportFile.SourceFile);
        }

        // Invalid json
        [Test]
        public void FailInvalidJson()
        {
            var (_, problems, _) = SetupCustomComponentsTests();

            var invalidJsonProblems = problems.Where(p => p.description == CustomComponentMarkupSystem.exceptionMessageInvalidJson).ToList();

            Assert.AreEqual(4, invalidJsonProblems.Count);

            Assert.IsTrue(Path.GetFullPath(invalidJsonProblems[0].location).EndsWith($"src{Path.DirectorySeparatorChar}invalid_json.ts"), $"{invalidJsonProblems[0].location} should end with \"src/invalid_json.ts\"");
            Assert.IsTrue(Path.GetFullPath(invalidJsonProblems[1].location).EndsWith($"src{Path.DirectorySeparatorChar}invalid_json.ts"), $"{invalidJsonProblems[0].location} should end with \"src/invalid_json.ts\"");
            Assert.IsTrue(Path.GetFullPath(invalidJsonProblems[2].location).EndsWith($"src{Path.DirectorySeparatorChar}invalid_json.ts"), $"{invalidJsonProblems[0].location} should end with \"src/invalid_json.ts\"");
            Assert.IsTrue(Path.GetFullPath(invalidJsonProblems[3].location).EndsWith($"src{Path.DirectorySeparatorChar}invalid_json.ts"), $"{invalidJsonProblems[0].location} should end with \"src/invalid_json.ts\"");
        }

        /*
// class is missing
#DCECOMP {
}

// class is not a string
#DCECOMP {
    "class": 5
}

// class is empty
#DCECOMP {
    "class": ""
}

// component is not a string
#DCECOMP {
    "class": "ClassAndComponent",
    "component": 5
}

// import file is not a string
#DCECOMP {
    "class": "ClassAndComponent",
    "import-file": 5
}

// property not a list (number)
#DCECOMP {
    "class": "ClassAndComponent",
    "properties": 5
}

// property not a list (object)
#DCECOMP {
    "class": "ClassAndComponent",
    "properties": {}
}

// property name is missing
#DCECOMP {
    "class": "ClassAndComponent",
    "properties": [
        {
            "type": "number"
        }
    ]
}

// property name is not a string
#DCECOMP {
    "class": "ClassAndComponent",
    "properties": [
        {
            "name": 5,
            "type": "number"
        }
    ]
}

// property name is empty
#DCECOMP {
    "class": "ClassAndComponent",
    "properties": [
        {
            "name": "",
            "type": "number"
        }
    ]
}

// property type is missing
#DCECOMP {
    "class": "ClassAndComponent",
    "properties": [
        {
            "name": "property1"
        }
    ]
}

// property type is not a string
#DCECOMP {
    "class": "ClassAndComponent",
    "properties": [
        {
            "name": "property1",
            "type": 5
        }
    ]
}

// property type is not a valid type
#DCECOMP {
    "class": "ClassAndComponent",
    "properties": [
        {
            "name": "property1",
            "type": "invalid"
        }
    ]
}

// property default is not a valid type (number)
#DCECOMP {
    "class": "ClassAndComponent",
    "properties": [
        {
            "name": "property1",
            "type": "number",
            "default": "invalid"
        }
    ]
}

// property default is not a valid type (string)
#DCECOMP {
    "class": "ClassAndComponent",
    "properties": [
        {
            "name": "property1",
            "type": "string",
            "default": 5
        }
    ]
}

// property default is not a valid type (vector3)
#DCECOMP {
    "class": "ClassAndComponent",
    "properties": [
        {
            "name": "property1",
            "type": "vector3",
            "default": "invalid"
        }
    ]
}
*/

        [Test]
        public void InterpretJson()
        {
            var (_, _, ccms) = SetupCustomComponentsTests();

            var classIsMissingString =
                @"{
                }";

            var classIsMissingException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(classIsMissingString), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageClassNotPresent, classIsMissingException.description);
            Assert.AreEqual("inline_test.ts:1:1", classIsMissingException.location);

            var classIsNotAStringString =
                @"{
                    ""class"": []
                }";

            var classIsNotAStringException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(classIsNotAStringString), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageClassWrongType, classIsNotAStringException.description);
            Assert.AreEqual("inline_test.ts:2:30", classIsNotAStringException.location);

            var classIsEmptyString =
                @"{
                    ""class"": """"
                }";
                
            var classIsEmptyException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(classIsEmptyString), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageClassWrongType, classIsEmptyException.description);
            Assert.AreEqual("inline_test.ts:2:30", classIsEmptyException.location);


        }
    }
}
