using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Highlight : MonoBehaviour
{
    //Settings
    [SerializeField]
    private List<MeshFilter> meshFilters = default;
    [SerializeField]


    public Highlight HighlightTarget(Hilightable target)
    {
        Highlight instance = Instantiate(gameObject, target.transform).GetComponent<Highlight>();

        Mesh targetMesh = target.OwnMeshFilter.sharedMesh;
        foreach (MeshFilter meshFilter in instance.meshFilters)
        {
            meshFilter.mesh = targetMesh;

            // If multiple sub meshes exist, add one Material per sub mesh
            if (meshFilter.mesh.subMeshCount > 1)
            {
                var mat = meshFilter.GetComponent<MeshRenderer>().materials[0];
                var mats = new List<Material>();

                for (int i = 0; i < meshFilter.mesh.subMeshCount; i++)
                {
                    mats.Add(mat);
                }

                meshFilter.GetComponent<MeshRenderer>().materials = mats.ToArray();
            }
        }

        return instance;
    }


    public void DestroyHighlight()
    {
        if (gameObject != null && gameObject)
            Destroy(gameObject);
    }
}
