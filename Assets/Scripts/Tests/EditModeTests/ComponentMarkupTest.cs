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
using UnityEngine.Rendering;
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

        [Test]
        public void RotationProperty()
        {
            var (availableComponentsState, _, ccms) = SetupCustomComponentsTests();

            var componentWithValidRotationEulerString =
                @"{
                    ""class"": ""ComponentWithValidRotationEuler"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": ""rotation"",
                            ""default"": "" E (20 ,40.5, 60) ""
                        }
                    ]
                }";

            ccms.MakeComponent(JObject.Parse(componentWithValidRotationEulerString).WithImportFile("src/inline_test"), "inline_test.ts");

            var componentWithValidRotationEulerComponent = availableComponentsState.GetComponentDefinitionByName("ComponentWithValidRotationEuler");
            
            Assert.AreEqual("ComponentWithValidRotationEuler", componentWithValidRotationEulerComponent.NameInCode);
            Assert.AreEqual("ComponentWithValidRotationEuler", componentWithValidRotationEulerComponent.NameOfSlot);

            Assert.AreEqual(1, componentWithValidRotationEulerComponent.properties.Count);

            var property = componentWithValidRotationEulerComponent.properties[0];
            Assert.AreEqual("property1", property.name);
            Assert.AreEqual(PropertyType.Quaternion, property.type);
            Assert.AreEqual(Quaternion.Euler(20,40.5f,60), property.defaultValue);



            var componentWithValidRotationQuaternionString =
                @"{
                    ""class"": ""ComponentWithValidRotationQuaternion"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": ""rotation"",
                            ""default"": "" Q (0.5, 0.5, 0.5, 0.5) ""
                        }
                    ]
                }";

            ccms.MakeComponent(JObject.Parse(componentWithValidRotationQuaternionString).WithImportFile("src/inline_test"), "inline_test.ts");

            var componentWithValidRotationQuaternionComponent = 
                availableComponentsState.GetComponentDefinitionByName("ComponentWithValidRotationQuaternion");

            Assert.AreEqual("ComponentWithValidRotationQuaternion", componentWithValidRotationQuaternionComponent.NameInCode);
            Assert.AreEqual("ComponentWithValidRotationQuaternion", componentWithValidRotationQuaternionComponent.NameOfSlot);

            Assert.AreEqual(1, componentWithValidRotationQuaternionComponent.properties.Count);

            property = componentWithValidRotationQuaternionComponent.properties[0];
            Assert.AreEqual("property1", property.name);
            Assert.AreEqual(PropertyType.Quaternion, property.type);
            Assert.AreEqual(new Quaternion(0.5f, 0.5f, 0.5f, 0.5f), property.defaultValue);



            var componentWithInvalidRotationString =
                @"{
                    ""class"": ""ComponentWithInvalidRotation"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": ""rotation"",
                            ""default"": ""invalid""
                        }
                    ]
                }";

            var componentWithInvalidRotationException= 
                Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => 
                ccms.MakeComponent(JObject.Parse(componentWithInvalidRotationString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageDefaultWrongTypeRotation, componentWithInvalidRotationException.description);
        }


        // test for property type asset:model
        [Test]
        public void AssetModelProperty()
        {
            var (availableComponentsState, _, ccms) = SetupCustomComponentsTests();

            /*
            // property type is asset:model
            #DCECOMP {
                "class": "ClassAndComponent",
                "properties": [
                    {
                        "name": "property1",
                        "type": "asset:model"
                    }
                ]
            }
            */
            var propertyTypeIsAssetModelString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": ""asset:model""
                        }
                    ]
                }";

            ccms.MakeComponent(JObject.Parse(propertyTypeIsAssetModelString).WithImportFile("src/inline_test"), "inline_test.ts");

            var component = availableComponentsState.GetComponentDefinitionByName("ClassAndComponent");

            Assert.AreEqual("ClassAndComponent", component.NameInCode);
            Assert.AreEqual("ClassAndComponent", component.NameOfSlot);

            Assert.AreEqual(1, component.properties.Count);

            var property = component.properties[0];

            Assert.AreEqual("property1", property.name);
            Assert.AreEqual(PropertyType.Asset, property.type);
            Assert.AreEqual(Guid.Empty, property.defaultValue);
            Assert.AreEqual(PropertyDefinition.Flags.ModelAssets, property.flags);

            // property type is asset:model and default value is valid
            var propertyTypeIsAssetModelAndDefaultValueIsValidString =
                @"{
                    ""class"": ""ClassAndComponent2"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": ""asset:model"",
                            ""default"": ""2a8c3e61-3b9e-4f9c-8c9c-1c9c3e61b9e4""
                        }
                    ]
                }";

            ccms.MakeComponent(JObject.Parse(propertyTypeIsAssetModelAndDefaultValueIsValidString).WithImportFile("src/inline_test"), "inline_test.ts");

            component = availableComponentsState.GetComponentDefinitionByName("ClassAndComponent2");

            Assert.AreEqual("ClassAndComponent2", component.NameInCode);
            Assert.AreEqual("ClassAndComponent2", component.NameOfSlot);

            Assert.AreEqual(1, component.properties.Count);

            property = component.properties[0];

            Assert.AreEqual("property1", property.name);
            Assert.AreEqual(PropertyType.Asset, property.type);
            Assert.AreEqual(new Guid("2a8c3e61-3b9e-4f9c-8c9c-1c9c3e61b9e4"), property.defaultValue);
            Assert.AreEqual(PropertyDefinition.Flags.ModelAssets, property.flags);

            // property type is asset:model and default value is invalid
            var propertyTypeIsAssetModelAndDefaultValueIsInvalidString =
                @"{
                    ""class"": ""ClassAndComponent3"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": ""asset:model"",
                            ""default"": ""invalid""
                        }
                    ]
                }";

            var propertyTypeIsAssetModelAndDefaultValueIsInvalidException =
                Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() =>
                    ccms.MakeComponent(JObject.Parse(propertyTypeIsAssetModelAndDefaultValueIsInvalidString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageDefaultWrongTypeAsset, propertyTypeIsAssetModelAndDefaultValueIsInvalidException.description);
        }

        // test for property type bool
        [Test]
        public void BoolProperty()
        {
            var (availableComponentsState, _, ccms) = SetupCustomComponentsTests();

            /*
            // property type is bool
            #DCECOMP {
                "class": "ClassAndComponent",
                "properties": [
                    {
                        "name": "property1",
                        "type": "bool"
                    }
                ]
            }
            */
            var propertyTypeIsBoolString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": ""bool""
                        }
                    ]
                }";

            ccms.MakeComponent(JObject.Parse(propertyTypeIsBoolString).WithImportFile("src/inline_test"), "inline_test.ts");

            var component = availableComponentsState.GetComponentDefinitionByName("ClassAndComponent");

            Assert.AreEqual("ClassAndComponent", component.NameInCode);
            Assert.AreEqual("ClassAndComponent", component.NameOfSlot);

            Assert.AreEqual(1, component.properties.Count);

            var property = component.properties[0];

            Assert.AreEqual("property1", property.name);
            Assert.AreEqual(PropertyType.Boolean, property.type);
            Assert.AreEqual(false, property.defaultValue);

            // property type is bool and default value is valid
            var propertyTypeIsBoolAndDefaultValueIsValidString =
                @"{
                    ""class"": ""ClassAndComponent2"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": ""bool"",
                            ""default"": true
                        }
                    ]
                }";

            ccms.MakeComponent(JObject.Parse(propertyTypeIsBoolAndDefaultValueIsValidString).WithImportFile("src/inline_test"), "inline_test.ts");

            component = availableComponentsState.GetComponentDefinitionByName("ClassAndComponent2");

            Assert.AreEqual("ClassAndComponent2", component.NameInCode);
            Assert.AreEqual("ClassAndComponent2", component.NameOfSlot);

            Assert.AreEqual(1, component.properties.Count);

            property = component.properties[0];

            Assert.AreEqual("property1", property.name);
            Assert.AreEqual(PropertyType.Boolean, property.type);
            Assert.AreEqual(true, property.defaultValue);

            // property type is bool and default value is invalid
            var propertyTypeIsBoolAndDefaultValueIsInvalidString =
                @"{
                    ""class"": ""ClassAndComponent3"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": ""bool"",
                            ""default"": ""invalid""
                        }
                    ]
                }";

            var propertyTypeIsBoolAndDefaultValueIsInvalidException =
                Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() =>
                    ccms.MakeComponent(JObject.Parse(propertyTypeIsBoolAndDefaultValueIsInvalidString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageDefaultWrongTypeBool, propertyTypeIsBoolAndDefaultValueIsInvalidException.description);
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


        [Test]
        public void InterpretJson()
        {
            var (_, _, ccms) = SetupCustomComponentsTests();
            /*
            // class is missing
            #DCECOMP {
            }
            */

            var classIsMissingString =
                @"{
                }";

            var classIsMissingException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(classIsMissingString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageClassNotPresent, classIsMissingException.description);
            Assert.AreEqual("inline_test.ts:1:1", classIsMissingException.location);

            /*
            // class is not a string
            #DCECOMP {
                "class": []
            }
            */
            var classIsNotAStringString =
                @"{
                    ""class"": []
                }";

            var classIsNotAStringException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(classIsNotAStringString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageClassWrongType, classIsNotAStringException.description);
            Assert.AreEqual("inline_test.ts:2:30", classIsNotAStringException.location);

            /*
            // class is empty
            #DCECOMP {
                "class": ""
            }
            */
            var classIsEmptyString =
                @"{
                    ""class"": """"
                }";

            var classIsEmptyException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(classIsEmptyString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageClassWrongType, classIsEmptyException.description);
            Assert.AreEqual("inline_test.ts:2:31", classIsEmptyException.location);

            /*
            // component is not a string
            #DCECOMP {
                "class": "ClassAndComponent",
                "component": []
            }
            */
            var componentIsNotAStringString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""component"": []
                }";

            var componentIsNotAStringException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(componentIsNotAStringString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageComponentWrongType, componentIsNotAStringException.description);
            Assert.AreEqual("inline_test.ts:3:34", componentIsNotAStringException.location);
            /*
            // import file is not a string
            #DCECOMP {
                "class": "ClassAndComponent",
                "import-file": []
            }
            */
            var importFileIsNotAStringString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""import-file"": []
                }";

            var importFileIsNotAStringException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(importFileIsNotAStringString), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageImportFileWrongType, importFileIsNotAStringException.description);
            Assert.AreEqual("inline_test.ts:3:36", importFileIsNotAStringException.location);
            /*
            // property not a list (number)
            #DCECOMP {
                "class": "ClassAndComponent",
                "properties": 5
            }
            */
            var propertiesIsNotAListNumberString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": 5
                }";

            var propertiesIsNotAListNumberException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(propertiesIsNotAListNumberString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessagePropertyListWrongType, propertiesIsNotAListNumberException.description);
            Assert.AreEqual("inline_test.ts:3:35", propertiesIsNotAListNumberException.location);
            /*
            // property not a list (object)
            #DCECOMP {
                "class": "ClassAndComponent",
                "properties": {}
            }
            */
            var propertiesIsNotAListObjectString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": {}
                }";

            var propertiesIsNotAListObjectException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(propertiesIsNotAListObjectString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessagePropertyListWrongType, propertiesIsNotAListObjectException.description);
            Assert.AreEqual("inline_test.ts:3:35", propertiesIsNotAListObjectException.location);
            /*
            // property name is missing
            #DCECOMP {
                "class": "ClassAndComponent",
                "properties": [
                    {
                        "type": "number"
                    }
                ]
            }
            */
            var propertyNameIsMissingString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": [
                        {
                            ""type"": ""number""
                        }
                    ]
                }";

            var propertyNameIsMissingException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(propertyNameIsMissingString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessagePropertyNameNotPresent, propertyNameIsMissingException.description);
            Assert.AreEqual("inline_test.ts:4:25", propertyNameIsMissingException.location);
            /*
            // property name is not a string
            #DCECOMP {
                "class": "ClassAndComponent",
                "properties": [
                    {
                        "name": [],
                        "type": "number"
                    }
                ]
            }
            */
            var propertyNameIsNotAStringString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": [
                        {
                            ""name"": [],
                            ""type"": ""number""
                        }
                    ]
                }";


            var propertyNameIsNotAStringException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(propertyNameIsNotAStringString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessagePropertyNameWrongType, propertyNameIsNotAStringException.description);
            Assert.AreEqual("inline_test.ts:5:37", propertyNameIsNotAStringException.location);
            /*
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
            */
            var propertyNameIsEmptyString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": [
                        {
                            ""name"": """",
                            ""type"": ""number""
                        }
                    ]
                }";

            var propertyNameIsEmptyException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(propertyNameIsEmptyString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessagePropertyNameWrongType, propertyNameIsEmptyException.description);
            Assert.AreEqual("inline_test.ts:5:38", propertyNameIsEmptyException.location);
            /*
            // property type is missing
            #DCECOMP {
                "class": "ClassAndComponent",
                "properties": [
                    {
                        "name": "property1"
                    }
                ]
            }
            */
            var propertyTypeIsMissingString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": [
                        {
                            ""name"": ""property1""
                        }
                    ]
                }";

            var propertyTypeIsMissingException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(propertyTypeIsMissingString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessagePropertyTypeNotPresent, propertyTypeIsMissingException.description);
            Assert.AreEqual("inline_test.ts:4:25", propertyTypeIsMissingException.location);
            /*
            // property type is not a string
            #DCECOMP {
                "class": "ClassAndComponent",
                "properties": [
                    {
                        "name": "property1",
                        "type": []
                    }
                ]
            }
            */
            var propertyTypeIsNotAStringString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": []
                        }
                    ]
                }";

            var propertyTypeIsNotAStringException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(propertyTypeIsNotAStringString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessagePropertyTypeWrongType, propertyTypeIsNotAStringException.description);
            Assert.AreEqual("inline_test.ts:6:37", propertyTypeIsNotAStringException.location);
            /*
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
            */
            var propertyTypeIsEmptyString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": """"
                        }
                    ]
                }";

            var propertyTypeIsEmptyException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(propertyTypeIsEmptyString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessagePropertyTypeWrongOption, propertyTypeIsEmptyException.description);
            Assert.AreEqual("inline_test.ts:6:38", propertyTypeIsEmptyException.location);
            /*
            // property type is not a valid type
            #DCECOMP {
                "class": "ClassAndComponent",
                "properties": [
                    {
                        "name": "property1",
                        "type": "not a valid type"
                    }
                ]
            }
            */
            var propertyTypeIsNotValidString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": ""not a valid type""
                        }
                    ]
                }";

            var propertyTypeIsNotValidException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(propertyTypeIsNotValidString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessagePropertyTypeWrongOption, propertyTypeIsNotValidException.description);
            Assert.AreEqual("inline_test.ts:6:54", propertyTypeIsNotValidException.location);
            /*
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
            */
            var defaultIsWrongType =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": ""number"",
                            ""default"": ""invalid""
                        }
                    ]
                }";

            var defaultIsWrongTypeException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(defaultIsWrongType).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageDefaultWrongTypeNumber, defaultIsWrongTypeException.description);
            Assert.AreEqual("inline_test.ts:7:48", defaultIsWrongTypeException.location);

            /*
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
            */
            var defaultIsWrongTypeString =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": ""string"",
                            ""default"": []
                        }
                    ]
                }";

            var defaultIsWrongTypeStringException = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(defaultIsWrongTypeString).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageDefaultWrongTypeString, defaultIsWrongTypeStringException.description);
            Assert.AreEqual("inline_test.ts:7:40", defaultIsWrongTypeStringException.location);

            /*
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
            var defaultIsWrongTypeVector3 =
                @"{
                    ""class"": ""ClassAndComponent"",
                    ""properties"": [
                        {
                            ""name"": ""property1"",
                            ""type"": ""vector3"",
                            ""default"": ""invalid""
                        }
                    ]
                }";

            var defaultIsWrongTypeVector3Exception = Assert.Throws<CustomComponentMarkupSystem.CustomComponentException>(() => ccms.MakeComponent(JObject.Parse(defaultIsWrongTypeVector3).WithImportFile("src/inline_test"), "inline_test.ts"));

            Assert.AreEqual(CustomComponentMarkupSystem.exceptionMessageDefaultWrongTypeVector3, defaultIsWrongTypeVector3Exception.description);
            Assert.AreEqual("inline_test.ts:7:48", defaultIsWrongTypeVector3Exception.location);
        }
    }
}
