using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GltfEntityAddView : MonoBehaviour
{
    [SerializeField]
    private GameObject _entityItemTemplate;

    // Start is called before the first frame update
    void Start()
    {
        var paths = Directory.GetFiles(SceneManager.DclProjectPath, "*.glb", SearchOption.AllDirectories);
        var projectUri = new Uri(SceneManager.DclProjectPath);

        foreach (Transform child in transform)
        {
            if (child.gameObject.TryGetComponent(out EntityItem item))
            {
                if (item.glbFileName != "")
                {
                    Destroy(child.gameObject);
                }
            }
        }

        foreach (var path in paths)
        {
            var relativePath = path.Replace(SceneManager.DclProjectPath+"\\", "").Replace("\\","/");

            if(relativePath.StartsWith("node_modules"))
                continue;
            
            var newItemObject = Instantiate(_entityItemTemplate, transform);
            var entityItem = newItemObject.GetComponent<EntityItem>();
            entityItem.glbFileName = relativePath;
            var pathParts = relativePath.Split('/');
            var fileName = pathParts[pathParts.Length - 1].Replace(".glb","");


            entityItem.text.text = fileName;
            entityItem.defaultName = fileName;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
