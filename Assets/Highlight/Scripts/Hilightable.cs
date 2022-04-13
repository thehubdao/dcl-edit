using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;


[RequireComponent(typeof(MeshFilter))]
public class Hilightable : MonoBehaviour
{
    private static List<Mesh> highlightableMeshes = new List<Mesh>();
    //public MeshFilter OwnMeshFilter { get; private set; }

    [CanBeNull] 
    public Mesh OwnSharedMesh { get; private set; } = null;


    void Awake()
    {
        if (TryGetComponent(out MeshFilter meshFilter))
            OwnSharedMesh = meshFilter.sharedMesh;
        
        if (OwnSharedMesh == null && TryGetComponent(out SkinnedMeshRenderer skinnedMeshRenderer))
            OwnSharedMesh = skinnedMeshRenderer.sharedMesh;

        MakeMeshHighlightable(OwnSharedMesh);
        
    }


    private void MakeMeshHighlightable(Mesh mesh)
    {
        if (!highlightableMeshes.Contains(mesh))
        {
            highlightableMeshes.Add(mesh);

            List<Vector3> smoothNormals = SmoothNormals(mesh);
            mesh.SetUVs(Highlighter.UVChannel, smoothNormals);
        }
    }


    //might behave incorrect on meshes with outside angles larger than 90 degree.
    private List<Vector3> SmoothNormals(Mesh mesh)
    {
        Vector3[] verts = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Group vertices by location
        var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

        // Copy normals to a new list
        var smoothNormals = new List<Vector3>(mesh.normals);

        // Average normals for grouped vertices
        foreach (var group in groups)
        {
            // Skip single vertices
            if (group.Count() == 1)
            {
                continue;
            }

            // Calculate the average normal
            Vector3 smoothNormal = Vector3.zero;

            List<Vector3> addedNormals = new List<Vector3>();
            float length = 1.0f;

            foreach (var pair in group)
            {
                Vector3 normal = mesh.normals[pair.Value];

                if (!addedNormals.Contains(normal))
                {
                    smoothNormal += normal;
                    length *= smoothingLehgthFactor(smoothNormal, normal);
                    addedNormals.Add(normal);
                }
            }

            smoothNormal = smoothNormal.normalized * length;

            // Assign smooth normal to each vertex
            foreach (var pair in group)
            {
                smoothNormals[pair.Value] = smoothNormal;
            }
        }

        return smoothNormals;
    }


    private float smoothingLehgthFactor(Vector3 a, Vector3 b)
    {
        float angle = Vector3.Angle(a, b) * Mathf.Deg2Rad;

        return 1 / Mathf.Cos(angle / 2);
    }
}
