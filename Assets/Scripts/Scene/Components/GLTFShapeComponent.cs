using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GLTFShapeComponent : EntityComponent
{
    class SpecificGltfShapeJson
    {
        public SpecificGltfShapeJson(GLTFShapeComponent sc)
        {
            glbPath = sc.glbPath;
        }

        public string glbPath;
    }

    public override string SpecificJson => JsonUtility.ToJson(new SpecificGltfShapeJson(this));
    public override void ApplySpecificJson(string jsonString)
    {
        var json = JsonUtility.FromJson<SpecificGltfShapeJson>(jsonString);
        glbPath = json.glbPath;
    }

    public string glbPath;

    public override string ComponentName => "GLTFShape";
    public override Ts GetTypeScript()
    {
        return new Ts( $"{entity.NameTsSymbol.ToCamelCase()}GltfShape", $"const {entity.NameTsSymbol.ToCamelCase()}GltfShape = new GLTFShape(\"{glbPath}\")\n" +
                                                                        $"{entity.NameTsSymbol.ToCamelCase()}GltfShape.withCollisions = true\n" +
                                                                        $"{entity.NameTsSymbol.ToCamelCase()}GltfShape.isPointerBlocker = true\n" +
                                                                        $"{entity.NameTsSymbol.ToCamelCase()}GltfShape.visible = true\n");
    }

    public override void Start()
    {
        base.Start();
        var componentRepresentation = Instantiate(ComponentRepresentationList.GltfShapeComponent, entity.componentsParent.transform);
        componentRepresentation.GetComponent<GltfComponentRepresentation>().UpdateVisuals(this);
    }

}
