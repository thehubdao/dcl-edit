using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScriptGenerator : MonoBehaviour
{
    public bool makeScript = false;

    // Start is called before the first frame update
    void Update()
    {
        if (makeScript)
        {
            MakeScript();
            makeScript = false;
        }
    }

    public struct ExposedVars
    {
        public string exposedAs;
        public string symbol;
    }

    public static string GetTypeScriptForEntity(Entity entity, ref List<ScriptGenerator.ExposedVars> exposed)
    {
        var script = $"const {entity.InternalSymbol} = new Entity(\"{entity.ShownName}\")\n";
        script += $"engine.addEntity({entity.InternalSymbol})\n";

        var exposedVar = new ScriptGenerator.ExposedVars
        {
            exposedAs = "entity",
            symbol = entity.InternalSymbol
        };
        exposed.Add(exposedVar);

        foreach (var component in entity.Components)
        {
            var componentTs = component.GetTypeScript();

            if(componentTs == null)
                continue;

            script += componentTs.Value.setup;
            script += $"{entity.InternalSymbol}.addComponentOrReplace({componentTs.Value.symbol})\n";

            var exposedComponentVar = new ScriptGenerator.ExposedVars
            {
                exposedAs = component.ComponentName,
                symbol = componentTs.Value.symbol
            };
            exposed.Add(exposedComponentVar);
        }

        return script;
    }

    public static void MakeScript()
    {
        Debug.Log("Making Script...");


        var fileWriter = new StreamWriter(DclSceneManager.DclProjectPath + "/" + ProjectData.generateScriptLocation, false);
        fileWriter.WriteLine("// FILE WAS GENERATED BY DCL-EDIT\n// DO NOT MODIFY\n");

        var allExposedVars = new Dictionary<string, List<ExposedVars>>();

        foreach (var entity in DclSceneManager.Entities)
        {
            var exposedVars = new List<ExposedVars>();
            fileWriter.WriteLine(GetTypeScriptForEntity(entity, ref exposedVars));

            if (entity.Exposed)
            {
                allExposedVars.Add(entity.ExposedSymbol, exposedVars);
            }

            fileWriter.WriteLine("// FILE WAS GENERATED BY DCL-EDIT\n// DO NOT MODIFY\n");
        }

        foreach (var entity in DclSceneManager.Entities)
        {
            var entityParent = entity.Parent as Entity;
            if (entityParent != null)
            {

                fileWriter.Write($"{entity.InternalSymbol}.setParent({entityParent.InternalSymbol})\n");
            }
        }

        fileWriter.WriteLine("\n\n");

        fileWriter.WriteLine("export let scene = {");

        foreach (var exposedVars in allExposedVars)
        {
            fileWriter.WriteLine("  " + exposedVars.Key + ":{");

            foreach (var variable in exposedVars.Value)
            {
                fileWriter.WriteLine($"    {variable.exposedAs}: {variable.symbol},");
            }

            fileWriter.WriteLine("  },");
        }

        fileWriter.WriteLine("}\n");


        fileWriter.Close();
    }
}
