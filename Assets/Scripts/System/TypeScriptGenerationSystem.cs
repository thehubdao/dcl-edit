using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using UnityEngine;
using Zenject;
using Debug = UnityEngine.Debug;

#pragma warning disable 0162

namespace Assets.Scripts.System
{
    public class TypeScriptGenerationSystem
    {
        // Dependencies
        private EditorState.SceneState _sceneState;
        private ExposeEntitySystem _exposeEntitySystem;
        private PathState _pathState;

        [Inject]
        private void Construct(EditorState.SceneState sceneState, ExposeEntitySystem exposeEntitySystem, PathState pathState)
        {
            _sceneState = sceneState;
            _exposeEntitySystem = exposeEntitySystem;
            _pathState = pathState;
        }

        private const bool _obfuscate = false;

        private struct GenerationInfo
        {
            public List<SceneInfo> GatheredSceneInfos;
            public List<UsedComponentInfo> UsedComponentInfos;
        }

        private struct SceneInfo
        {
            // The symbol used to refer to this scene. Aka the name of the scenes class
            public string Symbol;
            public List<EntityInfo> GatheredEntityInfos;
        }

        private struct EntityInfo
        {
            public Guid Id;
            public Guid ParentId;

            public string Name;
            public string InternalScriptSymbol;

            public bool IsExposed;
            public string ExposedSymbol;

            public List<EntityComponentInfo> GatheredComponentInfos;
        }

        private struct EntityComponentInfo
        {
            public string Symbol;
            public string InternalScriptSymbol;
            public string InEntitySymbol;
            public string WithTypeSymbol;

            public bool IsTransform;

            public List<PropertyInfo> GatheredPropertyInfos;
        }

        private struct UsedComponentInfo
        {
            public string Symbol;
            public string WithTypeSymbol;
            public string InEntitySymbol;
            public bool IsTransform;
        }

        private struct PropertyInfo
        {
            public string Symbol;
            public string Value;
        }


        public void GenerateTypeScript()
        {
            var generationInfo = GatherInfo();
            if (!generationInfo.HasValue)
            {
                Debug.LogError("Script generation: gathering info failed");
                return;
            }

            var script = GenerateActualScript(generationInfo.Value);

            var scriptsFolderPath = _pathState.ProjectPath + "/dcl-edit/build/scripts/";

            Directory.CreateDirectory(scriptsFolderPath);

            File.WriteAllText(scriptsFolderPath + "scenes.ts", script);

            Debug.Log(script);
        }

        private GenerationInfo? GatherInfo()
        {
            if (_sceneState.CurrentScene == null)
                return null;

            var generationInfo = new GenerationInfo()
            {
                GatheredSceneInfos = new List<SceneInfo>(),
                UsedComponentInfos = new List<UsedComponentInfo>()
            };

            // TODO: generate info for all scenes
            {
                var scene = _sceneState.CurrentScene;
                var uniqueSymbols = new List<string>();

                var sceneInfo = new SceneInfo()
                {
                    Symbol = _exposeEntitySystem.GenerateValidSymbol(scene.name),
                    GatheredEntityInfos = new List<EntityInfo>()
                };

                foreach (var entity in scene.AllEntities.Values)
                {
                    var internalEntitySymbol = _exposeEntitySystem.GenerateValidSymbol(_obfuscate ? "e" : entity.CustomName);

                    // make the internal symbol unique within the generated scene
                    {
                        var i = 0;
                        while (uniqueSymbols.Contains(internalEntitySymbol))
                        {
                            i++;
                            internalEntitySymbol = _exposeEntitySystem.GenerateValidSymbol((_obfuscate ? "e" : entity.CustomName) + i);
                        }
                    }

                    uniqueSymbols.Add(internalEntitySymbol);


                    var entityInfo = new EntityInfo()
                    {
                        Id = entity.Id,
                        ParentId = entity.ParentId,

                        Name = entity.ShownName,
                        InternalScriptSymbol = internalEntitySymbol,

                        IsExposed = entity.IsExposed,
                        ExposedSymbol = _exposeEntitySystem.ExposedName(entity),
                        GatheredComponentInfos = new List<EntityComponentInfo>()
                    };

                    foreach (var component in entity.Components)
                    {
                        // Create unique internal symbol
                        var internalEntityComponentSymbol = _obfuscate ? "c" : (internalEntitySymbol + component.NameInCode);

                        // make the internal symbol unique within the generated scene
                        {
                            var i = 0;
                            while (uniqueSymbols.Contains(internalEntityComponentSymbol))
                            {
                                i++;
                                internalEntityComponentSymbol = (_obfuscate ? "c" : (internalEntitySymbol + component.NameInCode)) + i;
                            }
                        }

                        uniqueSymbols.Add(internalEntityComponentSymbol);

                        // Create with type symbol
                        var withTypeSymbol = $"With{_exposeEntitySystem.GenerateValidSymbol(component.NameInCode)}";

                        // same as the internal script symbol but with the first letter in lower case
                        var inEntitySymbol =
                            char.ToLower(component.NameInCode[0])
                            + component.NameInCode.Substring(1);

                        var isTransform = component.NameInCode == "Transform";

                        // Generate component info
                        {
                            var componentInfo = new EntityComponentInfo()
                            {
                                Symbol = component.NameInCode,
                                WithTypeSymbol = withTypeSymbol,
                                InEntitySymbol = inEntitySymbol,
                                InternalScriptSymbol = internalEntityComponentSymbol,
                                IsTransform = isTransform,
                                GatheredPropertyInfos = new List<PropertyInfo>()
                            };

                            // Generate Property info
                            foreach (var property in component.Properties)
                            {
                                string value;

                                switch (property.Type)
                                {
                                    case DclComponent.DclComponentProperty.PropertyType.None:
                                        value = null;
                                        break;
                                    case DclComponent.DclComponentProperty.PropertyType.String:
                                        value = property.GetConcrete<string>().FixedValue;
                                        break;
                                    case DclComponent.DclComponentProperty.PropertyType.Int:
                                        value = property.GetConcrete<int>().FixedValue.ToString(CultureInfo.InvariantCulture);
                                        break;
                                    case DclComponent.DclComponentProperty.PropertyType.Float:
                                        value = property.GetConcrete<float>().FixedValue.ToString(CultureInfo.InvariantCulture);
                                        break;
                                    case DclComponent.DclComponentProperty.PropertyType.Boolean:
                                        value = property.GetConcrete<bool>().FixedValue ?
                                            "true" :
                                            "false";
                                        break;
                                    case DclComponent.DclComponentProperty.PropertyType.Vector3:
                                        var vector3 = property.GetConcrete<Vector3>().FixedValue;
                                        value = $"new Vector3({vector3.x.ToString(CultureInfo.InvariantCulture)}, {vector3.y.ToString(CultureInfo.InvariantCulture)}, {vector3.z.ToString(CultureInfo.InvariantCulture)})";
                                        break;
                                    case DclComponent.DclComponentProperty.PropertyType.Quaternion:
                                        var quaternion = property.GetConcrete<Quaternion>().FixedValue;
                                        value = $"new Quaternion({quaternion.x.ToString(CultureInfo.InvariantCulture)}, {quaternion.y.ToString(CultureInfo.InvariantCulture)}, {quaternion.z.ToString(CultureInfo.InvariantCulture)}, {quaternion.w.ToString(CultureInfo.InvariantCulture)})";
                                        break;
                                    case DclComponent.DclComponentProperty.PropertyType.Asset:
                                        value = null;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                                var propertyInfo = new PropertyInfo()
                                {
                                    Symbol = property.PropertyName,
                                    Value = value
                                };

                                componentInfo.GatheredPropertyInfos.Add(propertyInfo);
                            }

                            entityInfo.GatheredComponentInfos.Add(componentInfo);
                        }

                        // update the list of all used components
                        //if (entity.IsExposed)
                        {
                            if (!generationInfo
                                    .UsedComponentInfos
                                    .Select(ci => ci.Symbol)
                                    .Contains(component.NameInCode))
                            {
                                generationInfo.UsedComponentInfos.Add(new UsedComponentInfo()
                                {
                                    Symbol = component.NameInCode,
                                    WithTypeSymbol = withTypeSymbol,
                                    InEntitySymbol = inEntitySymbol,
                                    IsTransform = isTransform
                                });
                            }
                        }
                    }

                    sceneInfo.GatheredEntityInfos.Add(entityInfo);
                }

                generationInfo.GatheredSceneInfos.Add(sceneInfo);
            }

            return generationInfo;
        }

        private string GenerateActualScript(GenerationInfo generationInfo)
        {
            // Default script. This is always in the script
            // This contains the types for the dce entity and the dce scene
            var generatedScript = new StringBuilder();
            generatedScript.Append(@"export type DceScene = {
    sceneRoot: DceEntity

    /**
     * Shortcut for `sceneRoot.show()`
     */
    show: () => void;

    /**
     * Shortcut for `sceneRoot.hide()`
     */
    hide: () => void
}

export type DceEntity = {
    entity: Entity
    transform: Transform

    /**
     * Show this entity and all its children. This calls `engine.addEntity(entity)` internally.
     */
    show: () => void

    /**
     * Hide this entity and all its children. This calls `engine.removeEntity(entity)` internally.
     */
    hide: () => void
}

");

            if (_obfuscate)
            {
                generatedScript.AppendLine("function p(a: IEntity, b: IEntity){ a.setParent(b)}");
                generatedScript.AppendLine("function o(a: IEntity, b: any){ a.addComponent(b)}");
            }

            // WithComponent types
            /* Example:
                export type WithBoxShape = {
                    boxShape: BoxShape
                }
            */

            foreach (var componentInfo in generationInfo.UsedComponentInfos.Where(ci => !ci.IsTransform))
            {
                generatedScript.AppendLine($"export type {componentInfo.WithTypeSymbol} = {{".Indent(0));
                generatedScript.AppendLine($"{componentInfo.InEntitySymbol}: {componentInfo.Symbol}".Indent(1));
                generatedScript.AppendLine("}".Indent(0));
                generatedScript.AppendLine();
            }

            // Specific scene types
            /* Example:
                export type MyScene = DceScene & {
                    exposed: {
                        testCube: DceEntity & WithBoxShape,
                        testSphere: DceEntity & WithSphereShape & WithMyCustomComponent
                    }
                }
            */

            foreach (var sceneInfo in generationInfo.GatheredSceneInfos)
            {
                generatedScript.AppendLine($"export type {sceneInfo.Symbol} = DceScene & {{".Indent(0));
                generatedScript.AppendLine($"exposed: {{".Indent(1));

                foreach (var entityInfo in sceneInfo.GatheredEntityInfos.Where(info => info.IsExposed))
                {
                    generatedScript.Append($"{entityInfo.ExposedSymbol}: DceEntity".Indent(2));

                    foreach (var componentInfo in entityInfo
                                 .GatheredComponentInfos
                                 .Where(componentInfo => !componentInfo.IsTransform)) // Exclude Transform, because it already is included in DceEntity
                    {
                        generatedScript.Append($" & {componentInfo.WithTypeSymbol}");
                    }

                    generatedScript.AppendLine(",");
                }

                generatedScript.AppendLine("}".Indent(1));
                generatedScript.AppendLine("}".Indent(0));
            }

            generatedScript.AppendLine();

            // Scene factory
            /* Example:
            export class SceneFactory {
            |   static createMyScene(): MyScene {
            |   |   const rootEnt = new Entity()
            |   |   const rootTrans = new Transform()
            |   |   rootEnt.addComponent(rootTrans)
            |   |
            |   |   const testCubeEnt = new Entity()
            |   |   const testCubeTrans = new Transform()
            |   |   testCubeTrans.position = new Vector3(0, 0, 0)
            |   |   testCubeTrans.rotation = Quaternion.Euler(0, 0, 0)
            |   |   testCubeTrans.scale = new Vector3(1, 1, 1)
            |   |   testCubeEnt.addComponent(testCubeTrans)
            |   |   const testCubeBoxShape = new BoxShape()
            |   |   testCubeEnt.addComponent(testCubeBoxShape)
            |   |
            |   |   const testSphereEnt = new Entity()
            |   |   const testSphereTrans = new Transform()
            |   |   const testSphereSphereShape = new SphereShape()
            |   |   testSphereEnt.addComponent(testSphereTrans)
            |   |   testSphereEnt.addComponent(testSphereSphereShape)
            |   |
            |   |   testCubeEnt.setParent(rootEnt)
            |   |   testSphereEnt.setParent(rootEnt)
            |   |   otherScene.sceneRoot.entity.setParent(rootEnt)
            |   |
            |   |   engine.addEntity(rootEnt)
            |   |
            |   |   return {
            |   |   |   sceneRoot: {
            |   |   |   |   entity: rootEnt,
            |   |   |   |   transform: rootTrans,
            |   |   |   |   show() { engine.addEntity(this.entity) },
            |   |   |   |   hide() { engine.removeEntity(this.entity) }
            |   |   |   },
            |   |   |
            |   |   |   exposed: {
            |   |   |   |   testCube: {
            |   |   |   |   |   entity: testCubeEnt,
            |   |   |   |   |   transform: testCubeTrans,
            |   |   |   |   |   boxShape: testCubeBoxShape,
            |   |   |   |   |   show() { engine.addEntity(this.entity) },
            |   |   |   |   |   hide() { engine.removeEntity(this.entity) }
            |   |   |   |   },
            |   |   |   |   testSphere: {
            |   |   |   |   |   entity: testSphereEnt,
            |   |   |   |   |   transform: testSphereTrans,
            |   |   |   |   |   sphereShape: testSphereSphereShape,
            |   |   |   |   |   myCustomComponent: testSphereMyCustomComponent,
            |   |   |   |   |   show() { engine.addEntity(this.entity) },
            |   |   |   |   |   hide() { engine.removeEntity(this.entity) }
            |   |   |   |   }
            |   |   |   },
            |   |   |
            |   |   |   show() { this.sceneRoot.show() },
            |   |   |
            |   |   |   hide() { this.sceneRoot.hide() }
            |   |   }
            |   }
            }
         */
            generatedScript.AppendLine("export class SceneFactory {".Indent(0));

            foreach (var sceneInfo in generationInfo.GatheredSceneInfos)
            {
                generatedScript.AppendLine($"static create{sceneInfo.Symbol}(): {sceneInfo.Symbol} {{".Indent(1));

                const string rootEntitySymbol = _obfuscate ? "er" : "rootEntity";
                const string rootTransformSymbol = rootEntitySymbol + "Trans";

                // Root entity
                generatedScript.AppendLine($"const {rootEntitySymbol} = new Entity()".Indent(2));
                generatedScript.AppendLine($"const {rootTransformSymbol} = new Transform()".Indent(2));
                if (_obfuscate)
                {
                    generatedScript.AppendLine($"o({rootEntitySymbol},{rootTransformSymbol})".Indent(2));
                }
                else
                {
                    generatedScript.AppendLine($"{rootEntitySymbol}.addComponent({rootTransformSymbol})".Indent(2));
                }

                generatedScript.AppendLine();

                // other entities
                foreach (var entityInfo in sceneInfo.GatheredEntityInfos)
                {
                    generatedScript.AppendLine($"const {entityInfo.InternalScriptSymbol} = new Entity(\"{entityInfo.Name}\")".Indent(2));

                    // components
                    foreach (var componentInfo in entityInfo.GatheredComponentInfos)
                    {
                        // Temporary solution TODO: Change it
                        generatedScript.AppendLine(componentInfo.Symbol == "GLTFShape" ?
                            $"const {componentInfo.InternalScriptSymbol} = new {componentInfo.Symbol}(\"\")".Indent(2) : // use empty initializer until assets can be used here
                            $"const {componentInfo.InternalScriptSymbol} = new {componentInfo.Symbol}()".Indent(2));

                        // properties. Ignore properties without valid value
                        foreach (var propertyInfo in componentInfo.GatheredPropertyInfos.Where(pi => pi.Value != null))
                        {
                            generatedScript.AppendLine($"{componentInfo.InternalScriptSymbol}.{propertyInfo.Symbol} = {propertyInfo.Value}".Indent(2));
                        }

                        if (_obfuscate)
                        {
                            generatedScript.AppendLine($"o({entityInfo.InternalScriptSymbol},{componentInfo.InternalScriptSymbol})".Indent(2));
                        }
                        else
                        {
                            generatedScript.AppendLine($"{entityInfo.InternalScriptSymbol}.addComponent({componentInfo.InternalScriptSymbol})".Indent(2));
                        }
                    }

                    generatedScript.AppendLine();
                }

                // parents
                foreach (var entityInfo in sceneInfo.GatheredEntityInfos)
                {
                    if (entityInfo.ParentId == Guid.Empty)
                    {
                        if (_obfuscate)
                        {
                            generatedScript.AppendLine($"p({entityInfo.InternalScriptSymbol},{rootEntitySymbol})");
                        }
                        else
                        {
                            generatedScript.AppendLine($"{entityInfo.InternalScriptSymbol}.setParent({rootEntitySymbol})".Indent(2));
                        }
                    }
                    else
                    {
                        var parentsInternalScriptSymbol = sceneInfo
                            .GatheredEntityInfos
                            .Find(ei => ei.Id == entityInfo.ParentId)
                            .InternalScriptSymbol;

                        if (_obfuscate)
                        {
                            generatedScript.AppendLine($"p({entityInfo.InternalScriptSymbol},{parentsInternalScriptSymbol})");
                        }
                        else
                        {
                            generatedScript.AppendLine($"{entityInfo.InternalScriptSymbol}.setParent({parentsInternalScriptSymbol})".Indent(2));
                        }
                    }
                }

                // add root to engine
                generatedScript.AppendLine();
                generatedScript.AppendLine($"engine.addEntity({rootEntitySymbol})".Indent(2));
                generatedScript.AppendLine();

                // return object
                generatedScript.AppendLine("return {".Indent(2));

                // screne root
                /* Example:
                sceneRoot: {
                    entity: rootEnt,
                    transform: rootTrans,
                    show() { engine.addEntity(this.entity) },
                    hide() { engine.removeEntity(this.entity) }
                },
             */
                generatedScript.AppendLine("sceneRoot: {".Indent(3));
                generatedScript.AppendLine($"entity: {rootEntitySymbol},".Indent(4));
                generatedScript.AppendLine($"transform: {rootTransformSymbol},".Indent(4));
                generatedScript.AppendLine("show() { engine.addEntity(this.entity) },".Indent(4));
                generatedScript.AppendLine("hide() { engine.removeEntity(this.entity) }".Indent(4));
                generatedScript.AppendLine("},".Indent(3));

                // exposed
                /* Example:
                exposed: {
                    testCube: {
                        entity: testCubeEnt,
                        transform: testCubeTrans,
                        boxShape: testCubeBoxShape,
                        show() { engine.addEntity(this.entity) },
                        hide() { engine.removeEntity(this.entity) }
                    },
                    testSphere: {
                        entity: testSphereEnt,
                        transform: testSphereTrans,
                        sphereShape: testSphereSphereShape,
                        myCustomComponent: testSphereMyCustomComponent,
                        show() { engine.addEntity(this.entity) },
                        hide() { engine.removeEntity(this.entity) }
                    }
                },
             */

                generatedScript.AppendLine("exposed: {".Indent(3));

                // exposed entities
                foreach (var entityInfo in sceneInfo.GatheredEntityInfos.Where(ei => ei.IsExposed))
                {
                    generatedScript.AppendLine($"{entityInfo.ExposedSymbol}: {{".Indent(4));

                    generatedScript.AppendLine($"entity: {entityInfo.InternalScriptSymbol},".Indent(5));

                    // exposed components
                    foreach (var componentInfo in entityInfo.GatheredComponentInfos)
                    {
                        generatedScript.AppendLine($"{componentInfo.InEntitySymbol}: {componentInfo.InternalScriptSymbol},".Indent(5));
                    }

                    // show hide
                    generatedScript.AppendLine("show() { engine.addEntity(this.entity) },".Indent(5));
                    generatedScript.AppendLine("hide() { engine.removeEntity(this.entity) }".Indent(5));

                    generatedScript.AppendLine("},".Indent(4));
                }

                generatedScript.AppendLine("},".Indent(3));
                generatedScript.AppendLine();

                // Scene show hide

                generatedScript.AppendLine("show() { this.sceneRoot.show() },".Indent(3));
                generatedScript.AppendLine("hide() { this.sceneRoot.hide() }".Indent(3));

                generatedScript.AppendLine("}".Indent(2));
                generatedScript.AppendLine("}".Indent(1));
            }


            generatedScript.AppendLine("}".Indent(0));

            return generatedScript.ToString();
        }
    }
}

#pragma warning restore 0162
