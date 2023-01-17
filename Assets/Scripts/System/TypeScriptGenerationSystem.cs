using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using UnityEngine;
using Zenject;
using Debug = UnityEngine.Debug;

// Warning 0162 warns about unreachable code.
// The variable _obfuscate is currently constantly false.
// This variable is then used in an if statement, that leads to the unreachable code.
// This will be changed in the future.
#pragma warning disable 0162

namespace Assets.Scripts.System
{
    public class TypeScriptGenerationSystem
    {
        // Dependencies
        private ExposeEntitySystem exposeEntitySystem;
        private IPathState pathState;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        private void Construct(
            ExposeEntitySystem exposeEntitySystem,
            IPathState pathState,
            SceneManagerSystem sceneManagerSystem)
        {
            this.exposeEntitySystem = exposeEntitySystem;
            this.pathState = pathState;
            this.sceneManagerSystem = sceneManagerSystem;
        }

        private const bool obfuscate = false;

        private struct GenerationInfo
        {
            public List<SceneInfo> gatheredSceneInfos;
            public List<UsedComponentInfo> usedComponentInfos;
        }

        private struct SceneInfo
        {
            // The symbol used to refer to this scene. Aka the name of the scenes class
            public string symbol;
            public List<EntityInfo> gatheredEntityInfos;
        }

        private struct EntityInfo
        {
            public Guid id;
            public Guid parentId;

            public string name;
            public string internalScriptSymbol;

            public bool isExposed;
            public string exposedSymbol;

            public List<EntityComponentInfo> gatheredComponentInfos;
        }

        private struct EntityComponentInfo
        {
            public string symbol;
            public string internalScriptSymbol;
            public string inEntitySymbol;
            public string withTypeSymbol;

            public bool isTransform;

            public List<PropertyInfo> gatheredPropertyInfos;
        }

        private struct UsedComponentInfo
        {
            public string symbol;
            public string withTypeSymbol;
            public string inEntitySymbol;
            public bool isTransform;
        }

        private struct PropertyInfo
        {
            public string symbol;
            public string value;
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

            var scriptsFolderPath = pathState.ProjectPath + "/dcl-edit/build/scripts/";

            Directory.CreateDirectory(scriptsFolderPath);

            File.WriteAllText(scriptsFolderPath + "scenes.ts", script);
        }

        private GenerationInfo? GatherInfo()
        {
            SceneDirectoryState sceneDirectoryState = sceneManagerSystem.GetCurrentDirectoryState();

            if (sceneDirectoryState.currentScene == null)
            {
                return null;
            }

            var generationInfo = new GenerationInfo()
            {
                gatheredSceneInfos = new List<SceneInfo>(),
                usedComponentInfos = new List<UsedComponentInfo>()
            };

            // TODO: generate info for all scenes
            {
                var uniqueSymbols = new List<string>();

                var sceneInfo = new SceneInfo()
                {
                    symbol = exposeEntitySystem.GenerateValidSymbol(sceneDirectoryState.name),
                    gatheredEntityInfos = new List<EntityInfo>()
                };

                foreach (var entity in sceneDirectoryState.currentScene.AllEntities.Select(pair => pair.Value))
                {
                    var internalEntitySymbol = exposeEntitySystem.GenerateValidSymbol(obfuscate ? "e" : entity.CustomName);

                    // make the internal symbol unique within the generated scene
                    {
                        var i = 0;
                        while (uniqueSymbols.Contains(internalEntitySymbol))
                        {
                            i++;
                            internalEntitySymbol = exposeEntitySystem.GenerateValidSymbol((obfuscate ? "e" : entity.CustomName) + i);
                        }
                    }

                    uniqueSymbols.Add(internalEntitySymbol);


                    var entityInfo = new EntityInfo()
                    {
                        id = entity.Id,
                        parentId = entity.ParentId,

                        name = entity.ShownName,
                        internalScriptSymbol = internalEntitySymbol,

                        isExposed = entity.IsExposed,
                        exposedSymbol = exposeEntitySystem.ExposedName(entity),
                        gatheredComponentInfos = new List<EntityComponentInfo>()
                    };

                    foreach (var component in entity.Components)
                    {
                        // Create unique internal symbol
                        var internalEntityComponentSymbol = obfuscate ? "c" : (internalEntitySymbol + component.NameInCode);

                        // make the internal symbol unique within the generated scene
                        {
                            var i = 0;
                            while (uniqueSymbols.Contains(internalEntityComponentSymbol))
                            {
                                i++;
                                internalEntityComponentSymbol = (obfuscate ? "c" : (internalEntitySymbol + component.NameInCode)) + i;
                            }
                        }

                        uniqueSymbols.Add(internalEntityComponentSymbol);

                        // Create with type symbol
                        var withTypeSymbol = $"With{exposeEntitySystem.GenerateValidSymbol(component.NameInCode)}";

                        // same as the internal script symbol but with the first letter in lower case
                        var inEntitySymbol =
                            char.ToLower(component.NameInCode[0])
                            + component.NameInCode.Substring(1);

                        var isTransform = component.NameInCode == "Transform";

                        // Generate component info
                        {
                            var componentInfo = new EntityComponentInfo()
                            {
                                symbol = component.NameInCode,
                                withTypeSymbol = withTypeSymbol,
                                inEntitySymbol = inEntitySymbol,
                                internalScriptSymbol = internalEntityComponentSymbol,
                                isTransform = isTransform,
                                gatheredPropertyInfos = new List<PropertyInfo>()
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
                                    symbol = property.PropertyName,
                                    value = value
                                };

                                componentInfo.gatheredPropertyInfos.Add(propertyInfo);
                            }

                            entityInfo.gatheredComponentInfos.Add(componentInfo);
                        }

                        // update the list of all used components
                        //if (entity.IsExposed)
                        {
                            if (!generationInfo
                                    .usedComponentInfos
                                    .Select(ci => ci.symbol)
                                    .Contains(component.NameInCode))
                            {
                                generationInfo.usedComponentInfos.Add(new UsedComponentInfo()
                                {
                                    symbol = component.NameInCode,
                                    withTypeSymbol = withTypeSymbol,
                                    inEntitySymbol = inEntitySymbol,
                                    isTransform = isTransform
                                });
                            }
                        }
                    }

                    sceneInfo.gatheredEntityInfos.Add(entityInfo);
                }

                generationInfo.gatheredSceneInfos.Add(sceneInfo);
            }

            return generationInfo;
        }

        private string GenerateActualScript(GenerationInfo generationInfo)
        {
            // Default script. This is always in the script
            // This contains the types for the dce entity and the dce scene
            var generatedScript = new StringBuilder();
            generatedScript.Append(@"export type DceScene = {
    /**
     * The root entity of the scene. All entities in this scene are children of either this scene root entity, or of another entity in the scene
     */
    sceneRoot: DceEntity

    /**
     * Shows the scene with all its entities. Shortcut for `sceneRoot.show()`
     */
    show: () => void;

    /**
     * Hides the scene with all its entities. Shortcut for `sceneRoot.hide()`
     */
    hide: () => void
}

export type DceEntity = {
    /**
     * The Decentraland entity
     */
    entity: Entity

    /**
     * The Transform component of the entity. Although, it is not required by Decentraland, every DceEntity will have a Transform added
     */
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

            if (obfuscate)
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

            foreach (var componentInfo in generationInfo.usedComponentInfos.Where(ci => !ci.isTransform))
            {
                generatedScript.AppendLine($"export type {componentInfo.withTypeSymbol} = {{".Indent(0));
                generatedScript.AppendLine($"{componentInfo.inEntitySymbol}: {componentInfo.symbol}".Indent(1));
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

            foreach (var sceneInfo in generationInfo.gatheredSceneInfos)
            {
                generatedScript.AppendLine($"export type {sceneInfo.symbol} = DceScene & {{".Indent(0));
                generatedScript.AppendLine($"exposed: {{".Indent(1));

                foreach (var entityInfo in sceneInfo.gatheredEntityInfos.Where(info => info.isExposed))
                {
                    generatedScript.Append($"{entityInfo.exposedSymbol}: DceEntity".Indent(2));

                    foreach (var componentInfo in entityInfo
                                 .gatheredComponentInfos
                                 .Where(componentInfo => !componentInfo.isTransform)) // Exclude Transform, because it already is included in DceEntity
                    {
                        generatedScript.Append($" & {componentInfo.withTypeSymbol}");
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
            |   static createMyScene(rootEntity: Entity | null = null): MyScene {
            |   |   if (rootEntity == null) {
            |   |   |   rootEntity = new Entity()
            |   |   |   const rootEntityTrans = new Transform()
            |   |   |   rootEntity.addComponent(rootEntityTrans)
            |   |   } else {
            |   |   |   if (!rootEntity.hasComponent(Transform)) {
            |   |   |   |   rootEntity.addComponent(new Transform)
            |   |   |   }
            |   |   }
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
            |   |   |   |   |   transform: rootEntity.getComponent(Transform),
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

            foreach (var sceneInfo in generationInfo.gatheredSceneInfos)
            {
                generatedScript.AppendLine("/**".Indent(1));
                generatedScript.AppendLine($" * Creates a new instance of the scene {sceneInfo.symbol}".Indent(1));
                generatedScript.AppendLine(" * @param rootEntity specify a root entity for the newly created scene. If null, a new Entity will be generated as the root".Indent(1));
                generatedScript.AppendLine(" */".Indent(1));

                generatedScript.AppendLine($"static create{sceneInfo.symbol}(rootEntity: Entity | null = null): {sceneInfo.symbol} {{".Indent(1));

                const string rootEntitySymbol = "rootEntity";
                const string rootTransformSymbol = rootEntitySymbol + "Trans";

                // Root entity
                generatedScript.AppendLine($"if ({rootEntitySymbol} == null) {{".Indent(2));
                generatedScript.AppendLine($"{rootEntitySymbol} = new Entity()".Indent(3));
                generatedScript.AppendLine($"const {rootTransformSymbol} = new Transform()".Indent(3));
                if (obfuscate)
                {
                    generatedScript.AppendLine($"o({rootEntitySymbol},{rootTransformSymbol})");
                }
                else
                {
                    generatedScript.AppendLine($"{rootEntitySymbol}.addComponent({rootTransformSymbol})".Indent(3));
                }

                generatedScript.AppendLine("} else {".Indent(2));
                generatedScript.AppendLine($"if (!{rootEntitySymbol}.hasComponent(Transform)) {{".Indent(3));
                if (obfuscate)
                {
                    generatedScript.AppendLine($"o({rootEntitySymbol},new Transform)");
                }
                else
                {
                    generatedScript.AppendLine($"{rootEntitySymbol}.addComponent(new Transform)".Indent(4));
                }

                generatedScript.AppendLine($"}}".Indent(3));
                generatedScript.AppendLine("}".Indent(2));


                generatedScript.AppendLine();

                // other entities
                foreach (var entityInfo in sceneInfo.gatheredEntityInfos)
                {
                    generatedScript.AppendLine($"const {entityInfo.internalScriptSymbol} = new Entity(\"{entityInfo.name}\")".Indent(2));

                    // components
                    foreach (var componentInfo in entityInfo.gatheredComponentInfos)
                    {
                        // Temporary solution TODO: Change it
                        generatedScript.AppendLine(componentInfo.symbol == "GLTFShape" ?
                            $"const {componentInfo.internalScriptSymbol} = new {componentInfo.symbol}(\"\")".Indent(2) : // use empty initializer until assets can be used here
                            $"const {componentInfo.internalScriptSymbol} = new {componentInfo.symbol}()".Indent(2));

                        // properties. Ignore properties without valid value
                        foreach (var propertyInfo in componentInfo.gatheredPropertyInfos.Where(pi => pi.value != null))
                        {
                            generatedScript.AppendLine($"{componentInfo.internalScriptSymbol}.{propertyInfo.symbol} = {propertyInfo.value}".Indent(2));
                        }

                        if (obfuscate)
                        {
                            generatedScript.AppendLine($"o({entityInfo.internalScriptSymbol},{componentInfo.internalScriptSymbol})".Indent(2));
                        }
                        else
                        {
                            generatedScript.AppendLine($"{entityInfo.internalScriptSymbol}.addComponent({componentInfo.internalScriptSymbol})".Indent(2));
                        }
                    }

                    generatedScript.AppendLine();
                }

                // parents
                foreach (var entityInfo in sceneInfo.gatheredEntityInfos)
                {
                    if (entityInfo.parentId == Guid.Empty)
                    {
                        if (obfuscate)
                        {
                            generatedScript.AppendLine($"p({entityInfo.internalScriptSymbol},{rootEntitySymbol})");
                        }
                        else
                        {
                            generatedScript.AppendLine($"{entityInfo.internalScriptSymbol}.setParent({rootEntitySymbol})".Indent(2));
                        }
                    }
                    else
                    {
                        var parentsInternalScriptSymbol = sceneInfo
                            .gatheredEntityInfos
                            .Find(ei => ei.id == entityInfo.parentId)
                            .internalScriptSymbol;

                        if (obfuscate)
                        {
                            generatedScript.AppendLine($"p({entityInfo.internalScriptSymbol},{parentsInternalScriptSymbol})");
                        }
                        else
                        {
                            generatedScript.AppendLine($"{entityInfo.internalScriptSymbol}.setParent({parentsInternalScriptSymbol})".Indent(2));
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
                generatedScript.AppendLine($"transform: {rootEntitySymbol}.getComponent(Transform),".Indent(4));
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
                foreach (var entityInfo in sceneInfo.gatheredEntityInfos.Where(ei => ei.isExposed))
                {
                    generatedScript.AppendLine($"{entityInfo.exposedSymbol}: {{".Indent(4));

                    generatedScript.AppendLine($"entity: {entityInfo.internalScriptSymbol},".Indent(5));

                    // exposed components
                    foreach (var componentInfo in entityInfo.gatheredComponentInfos)
                    {
                        generatedScript.AppendLine($"{componentInfo.inEntitySymbol}: {componentInfo.internalScriptSymbol},".Indent(5));
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
