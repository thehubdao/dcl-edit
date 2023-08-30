using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using static Assets.Scripts.SceneState.DclComponent.DclComponentProperty.PropertyDefinition.Flags;
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
        private SceneManagerState sceneManagerState;
        private SceneManagerSystem sceneManagerSystem;
        private AssetManagerSystem assetManagerSystem;
        private AvailableComponentsState availableComponentsState;

        [Inject]
        private void Construct(
            ExposeEntitySystem exposeEntitySystem,
            IPathState pathState,
            SceneManagerState sceneManagerState,
            SceneManagerSystem sceneManagerSystem,
            AssetManagerSystem assetManagerSystem,
            AvailableComponentsState availableComponentsState)
        {
            this.exposeEntitySystem = exposeEntitySystem;
            this.pathState = pathState;
            this.sceneManagerState = sceneManagerState;
            this.sceneManagerSystem = sceneManagerSystem;
            this.assetManagerSystem = assetManagerSystem;
            this.availableComponentsState = availableComponentsState;
        }

        private const bool obfuscate = false;

        private struct GenerationInfo
        {
            public List<SceneInfo> gatheredSceneInfos;
            public List<UsedComponentInfo> usedComponentInfos;
            public Dictionary<Guid, string> neededAssets;
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
            public enum SpecialComponent
            {
                NotSpecial,
                Transform,
                ChildScene,

                // shapes
                BoxRenderer,
                SphereRenderer,
                PlaneRenderer,
                CylinderRenderer,
                ConeRenderer,

                // collider
                BoxCollider,
                SphereCollider,
                PlaneCollider,
                CylinderCollider,
                ConeCollider,
            }

            public string symbol;
            public string internalScriptSymbol;
            public string inEntitySymbol;
            public string withTypeSymbol;

            public SpecialComponent specialComponent;

            public List<PropertyInfo> gatheredPropertyInfos;
            public bool shouldGenerateInitFunction;
        }

        private struct UsedComponentInfo
        {
            public string symbol;
            public string withTypeSymbol;
            public string inEntitySymbol;

            [CanBeNull]
            public string sourceFile;

            public EntityComponentInfo.SpecialComponent specialComponent;
        }

        private struct PropertyInfo
        {
            public string symbol;
            public string value;
            public bool isConstructorParameter;
        }


        public async Task<bool> GenerateTypeScript()
        {
            try
            {
                var generationInfo = await GatherInfo();
                if (!generationInfo.HasValue)
                {
                    Debug.LogError("Script generation: gathering info failed");
                    return false;
                }

                var script = GenerateActualScript(generationInfo.Value);

                var scriptsFolderPath = pathState.ProjectPath + "/dcl-edit/build/scripts/";

                Directory.CreateDirectory(scriptsFolderPath);

                File.WriteAllText(scriptsFolderPath + "scenes.ts", script);

                Debug.Log("Script generation done");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        private async Task<GenerationInfo?> GatherInfo()
        {
            var generationInfo = new GenerationInfo()
            {
                gatheredSceneInfos = new List<SceneInfo>(),
                usedComponentInfos = new List<UsedComponentInfo>(),
                neededAssets = new Dictionary<Guid, string>()
            };

            // Gather scene names
            var sceneNames = new Dictionary<Guid, string>();

            foreach (var sceneDirectoryState in sceneManagerState.allSceneDirectoryStates)
            {
                string name;
                var i = 0;
                do
                {
                    i++;
                    name = exposeEntitySystem.GenerateValidSymbol(sceneDirectoryState.name) + (i > 1 ? i.ToString() : "");
                } while (sceneNames.ContainsValue(name));

                sceneNames.Add(sceneDirectoryState.id, name);
            }

            foreach (var sceneDirectoryState in sceneManagerState.allSceneDirectoryStates)
            {
                generationInfo.gatheredSceneInfos.Add(await GatherSceneInfo(sceneDirectoryState, generationInfo.usedComponentInfos, sceneNames, generationInfo.neededAssets));
            }

            return generationInfo;
        }

        private async Task<SceneInfo> GatherSceneInfo(SceneDirectoryState sceneDirectoryState, ICollection<UsedComponentInfo> usedComponentInfos, IReadOnlyDictionary<Guid, string> sceneNames, Dictionary<Guid, string> neededAssets)
        {
            var uniqueSymbols = new List<string>();

            var sceneInfo = new SceneInfo()
            {
                symbol = sceneNames[sceneDirectoryState.id],
                gatheredEntityInfos = new List<EntityInfo>()
            };

            var dclScene = sceneManagerSystem.GetScene(sceneDirectoryState.id);

            foreach (var entity in dclScene.AllEntities.Select(pair => pair.Value))
            {
                sceneInfo.gatheredEntityInfos.Add(await GatherEntityInfo(entity, usedComponentInfos, uniqueSymbols, sceneNames, neededAssets));
            }

            return sceneInfo;
        }

        private async Task<EntityInfo> GatherEntityInfo(DclEntity entity, ICollection<UsedComponentInfo> usedComponentInfos, ICollection<string> uniqueSymbols, IReadOnlyDictionary<Guid, string> sceneNames, Dictionary<Guid, string> neededAssets)
        {
            // make the internal symbol unique within the generated scene
            var i = 0;
            string internalEntitySymbol;
            do
            {
                i++;
                internalEntitySymbol = exposeEntitySystem.GenerateValidSymbol((obfuscate ? "e" : ("ent4_" + entity.CustomName)) + i);
            } while (uniqueSymbols.Contains(internalEntitySymbol));


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
                entityInfo.gatheredComponentInfos.Add(await GatherComponentInfo(component, internalEntitySymbol, usedComponentInfos, uniqueSymbols, sceneNames, neededAssets));
            }

            return entityInfo;
        }

        private async Task<EntityComponentInfo> GatherComponentInfo(DclComponent component, string internalEntitySymbol, ICollection<UsedComponentInfo> usedComponentInfos, ICollection<string> uniqueSymbols, IReadOnlyDictionary<Guid, string> sceneNames, Dictionary<Guid, string> neededAssets)
        {
            var specialComponent = component.NameInCode switch
            {
                "Transform" => EntityComponentInfo.SpecialComponent.Transform,
                "Scene" => EntityComponentInfo.SpecialComponent.ChildScene,
                _ => EntityComponentInfo.SpecialComponent.NotSpecial
            };

            // Create unique internal symbol
            var internalEntityComponentSymbol = obfuscate ? "c" : (internalEntitySymbol + component.NameInCode);


            if (specialComponent == EntityComponentInfo.SpecialComponent.ChildScene)
            {
                var sceneComponent = new DclSceneComponent(component);

                var sceneName = sceneNames[sceneComponent.sceneId.Value];

                {
                    if (!usedComponentInfos
                            .Select(ci => ci.symbol)
                            .Contains(sceneName))
                    {
                        usedComponentInfos.Add(new UsedComponentInfo()
                        {
                            symbol = sceneName,
                            withTypeSymbol = $"WithChildScene{sceneName}",
                            inEntitySymbol = "childScene",
                            specialComponent = specialComponent
                        });
                    }
                }

                return new EntityComponentInfo()
                {
                    symbol = $"{sceneName}",
                    withTypeSymbol = $"WithChildScene{sceneName}",
                    inEntitySymbol = "childScene",
                    internalScriptSymbol = internalEntityComponentSymbol,
                    specialComponent = specialComponent,
                    gatheredPropertyInfos = new List<PropertyInfo>(),
                    shouldGenerateInitFunction = false
                };
            }

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


            // Generate component info

            var componentInfo = new EntityComponentInfo()
            {
                symbol = component.NameInCode,
                withTypeSymbol = withTypeSymbol,
                inEntitySymbol = inEntitySymbol,
                internalScriptSymbol = internalEntityComponentSymbol,
                specialComponent = specialComponent,
                gatheredPropertyInfos = new List<PropertyInfo>(),
                shouldGenerateInitFunction = true
            };

            // Generate Property info
            foreach (var property in component.Properties)
            {
                componentInfo.gatheredPropertyInfos.Add(await GatherPropertyInfo(component, property, neededAssets));
            }

            // find the correct component definition
            var componentDefinition = availableComponentsState.GetComponentDefinitionByName(component.NameInCode);

            // update the list of all used components
            //if (entity.IsExposed)
            {
                if (!usedComponentInfos
                        .Select(ci => ci.symbol)
                        .Contains(component.NameInCode))
                {
                    usedComponentInfos.Add(new UsedComponentInfo()
                    {
                        symbol = component.NameInCode,
                        withTypeSymbol = withTypeSymbol,
                        inEntitySymbol = inEntitySymbol,
                        sourceFile = componentDefinition.SourceFile,
                        specialComponent = specialComponent
                    });
                }
            }

            return componentInfo;
        }

        private async Task<PropertyInfo> GatherPropertyInfo(DclComponent component, DclComponent.DclComponentProperty property, Dictionary<Guid, string> neededAssets)
        {
            string value;


            switch (property.Type)
            {
                case DclComponent.DclComponentProperty.PropertyType.None:
                    value = null;
                    break;
                case DclComponent.DclComponentProperty.PropertyType.String:
                    value = $"\"{property.GetConcrete<string>().FixedValue}\"";
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
                    value = $"Vector3.create({vector3.x.ToString(CultureInfo.InvariantCulture)}, {vector3.y.ToString(CultureInfo.InvariantCulture)}, {vector3.z.ToString(CultureInfo.InvariantCulture)})";
                    break;
                case DclComponent.DclComponentProperty.PropertyType.Quaternion:
                    var quaternion = property.GetConcrete<Quaternion>().FixedValue;
                    value = $"Quaternion.create({quaternion.x.ToString(CultureInfo.InvariantCulture)}, {quaternion.y.ToString(CultureInfo.InvariantCulture)}, {quaternion.z.ToString(CultureInfo.InvariantCulture)}, {quaternion.w.ToString(CultureInfo.InvariantCulture)})";
                    break;
                case DclComponent.DclComponentProperty.PropertyType.Asset:
                    value = $"\"{await BuildOrGetAsset(property.GetConcrete<Guid>().FixedValue, neededAssets)}\"";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return new PropertyInfo()
            {
                symbol = property.PropertyName,
                value = value,
                isConstructorParameter = (availableComponentsState.GetComponentDefinitionByName(component.NameInCode).GetPropertyDefinitionByName(property.PropertyName).flags & ParseInConstructor) > 0
            };
        }

        private async Task<string> BuildOrGetAsset(Guid id, Dictionary<Guid, string> neededAssets)
        {
            if (neededAssets.TryGetValue(id, out var asset))
            {
                return asset;
            }

            var assetPath = await assetManagerSystem.CopyAssetTo(id, "Some path");
            neededAssets.Add(id, assetPath);
            return assetPath;
        }

        const string rootEntitySymbol = "rootEntity";

        private string GenerateActualScript(GenerationInfo generationInfo)
        {
            var generatedScript = new StringBuilder();
            string doNotModify = ("\n// THIS FILE WAS GENERATED BY DCL-EDIT!\n// DO NOT MODIFY!\n\n");

            // add imports
            generatedScript.AppendLine("import { engine, Transform, GltfContainer, Entity, MeshRenderer } from '@dcl/sdk/ecs'");
            generatedScript.AppendLine("import { Vector3, Quaternion } from '@dcl/sdk/math'");

            foreach (var componentInfo in generationInfo.usedComponentInfos.Where(c => c.sourceFile != null))
            {
                generatedScript.AppendLine($"import {{ {componentInfo.symbol} }} from \"{componentInfo.sourceFile}\"");
            }

            generatedScript.AppendLine("export class SceneFactory {".Indent(0));

            foreach (var sceneInfo in generationInfo.gatheredSceneInfos)
            {
                generatedScript.AppendLine("/**".Indent(1));
                generatedScript.AppendLine($" * Creates a new instance of the scene {sceneInfo.symbol}".Indent(1));
                generatedScript.AppendLine(" * @param rootEntity specify a root entity for the newly created scene. If null, a new Entity will be generated as the root".Indent(1));
                generatedScript.AppendLine(" */".Indent(1));

                generatedScript.AppendLine($"static create{sceneInfo.symbol}(rootEntity: Entity | null = null) {{".Indent(1));


                // Root entity 
                generatedScript.AppendLine($"if ({rootEntitySymbol} == null) {{".Indent(2));
                generatedScript.AppendLine($"{rootEntitySymbol} = engine.addEntity()".Indent(3));

                generatedScript.AppendLine("}".Indent(2));

                generatedScript.AppendLine($"if(!Transform.has({rootEntitySymbol})){{".Indent(2));
                generatedScript.AppendLine($"Transform.create({rootEntitySymbol})".Indent(3));
                generatedScript.AppendLine("}".Indent(2));

                generatedScript.Append(doNotModify);


                // add all entities first
                foreach (var entityInfo in sceneInfo.gatheredEntityInfos)
                {
                    //--------------------------------------------------------------
                    generatedScript.AppendLine($"let {entityInfo.internalScriptSymbol} = engine.addEntity()".Indent(2));
                }

                generatedScript.Append(doNotModify);

                // add entity components
                foreach (var entityInfo in sceneInfo.gatheredEntityInfos)
                {
                    // components
                    foreach (var componentInfo in entityInfo.gatheredComponentInfos)
                    {
                        GenerateComponent(sceneInfo, entityInfo, componentInfo, generatedScript);
                    }

                    generatedScript.Append(doNotModify);
                }

                GenerateReturn(sceneInfo, generatedScript);

                generatedScript.AppendLine("}".Indent(1));
            }

            generatedScript.AppendLine("}".Indent(0));

            return generatedScript.ToString();
        }

        private void GenerateComponent(SceneInfo sceneInfo, EntityInfo entityInfo, EntityComponentInfo componentInfo, StringBuilder generatedScript)
        {
            // Special components
            switch (componentInfo.specialComponent)
            {
                case EntityComponentInfo.SpecialComponent.Transform:
                    generatedScript.AppendLine($"{componentInfo.symbol}.create({entityInfo.internalScriptSymbol}, {{".Indent(2));

                    // Transform properties
                    foreach (var propertyInfo in componentInfo.gatheredPropertyInfos)
                    {
                        generatedScript.AppendLine($"{propertyInfo.symbol}: {propertyInfo.value},".Indent(3));
                    }

                    // Parent
                    var parentSymbol = entityInfo.parentId == Guid.Empty ?
                        rootEntitySymbol :
                        sceneInfo.gatheredEntityInfos.First(ei => ei.id == entityInfo.parentId).internalScriptSymbol;

                    generatedScript.AppendLine($"parent: {parentSymbol},".Indent(3));

                    generatedScript.AppendLine("})".Indent(2));
                    break;

                case EntityComponentInfo.SpecialComponent.ChildScene:
                    generatedScript.AppendLine($"let {InternalSceneInSceneSymbol(entityInfo.internalScriptSymbol)} = SceneFactory.create{componentInfo.symbol}({entityInfo.internalScriptSymbol})".Indent(2));
                    break;

                case EntityComponentInfo.SpecialComponent.NotSpecial:
                default:
                    generatedScript.AppendLine($"{componentInfo.symbol}.create({entityInfo.internalScriptSymbol}, {{".Indent(2));

                    // properties. Ignore properties without valid value
                    foreach (var propertyInfo in componentInfo.gatheredPropertyInfos.Where(pi => pi.value != null))
                    {
                        generatedScript.AppendLine($"{propertyInfo.symbol}: {propertyInfo.value},".Indent(3));
                    }

                    generatedScript.AppendLine("})".Indent(2));
                    break;
            }
        }

        private void GenerateReturn(SceneInfo sceneInfo, StringBuilder generatedScript)
        {
            generatedScript.AppendLine("return {".Indent(2));

            generatedScript.AppendLine($"$root: {rootEntitySymbol},".Indent(3));
            foreach (var entityInfo in sceneInfo.gatheredEntityInfos.Where(ei => ei.isExposed))
            {
                var isSceneInSceneEntity = entityInfo.gatheredComponentInfos.Any(ci => ci.specialComponent == EntityComponentInfo.SpecialComponent.ChildScene);
                var internalSymbol =
                    isSceneInSceneEntity ?
                        InternalSceneInSceneSymbol(entityInfo.internalScriptSymbol) :
                        entityInfo.internalScriptSymbol;
                generatedScript.AppendLine($"{entityInfo.exposedSymbol}: {internalSymbol},".Indent(3));
            }

            generatedScript.AppendLine("}".Indent(2));
        }

        private string InternalSceneInSceneSymbol(string entitySymbol)
        {
            return entitySymbol + "Scene";
        }

        void OnScriptHotReload()
        {
            GenerateTypeScript();
        }
    }
}

#pragma warning restore 0162
