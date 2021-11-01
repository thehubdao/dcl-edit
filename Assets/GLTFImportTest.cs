using System.Collections;
using System.Collections.Generic;
using Siccity.GLTFUtility;
using UnityEngine;

public class GLTFImportTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() 
    {
        Importer.LoadFromFileAsync("F:\\Data\\Decentraland\\Pong\\f4a8e154-ac7b-43ac-b1c9-d0fc1e21406d\\Vase_04\\Vase_04.glb",new ImportSettings(),((o, clips) => {}));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
