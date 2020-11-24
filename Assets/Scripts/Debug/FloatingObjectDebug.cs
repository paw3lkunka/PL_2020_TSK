using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FloatingObjectDebug : MonoBehaviour
{
    [SerializeField]private GameObject floatingObject;  
    private FloatingObjectMeshParser meshParser;
    private FloatingObjectPhysics physics;
    
    private Mesh submergedMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>() as MeshFilter;
        meshRenderer = gameObject.GetComponent<MeshRenderer>() as MeshRenderer;

        meshParser = floatingObject.GetComponent<FloatingObjectMeshParser>() as FloatingObjectMeshParser;
        physics = floatingObject.GetComponent<FloatingObjectPhysics>() as FloatingObjectPhysics;

        submergedMesh = new Mesh();
        meshFilter.mesh = submergedMesh;
        
    }

    private void FixedUpdate()
    {
        GenerateSubmergedMesh();
    }

    private void GenerateSubmergedMesh()
    {
        Triangle[] submergedTriangles = meshParser.submerged.ToArray();
        HashSet<Vector3> submergedVerticesSet = new HashSet<Vector3>();

        for(int i = 0; i < meshParser.submerged.Count; i++)
        {
            submergedVerticesSet.UnionWith(new Vector3[] {submergedTriangles[i].a, submergedTriangles[i].b, submergedTriangles[i].c});
        }

        Vector3[] submergedVertices = submergedVerticesSet.ToArray();
        int[] submergedIndices = new int[6 * submergedTriangles.Length];

        for(int i = 0; i < submergedTriangles.Length; i++)
        {
            submergedIndices[3 * i] = Array.IndexOf(submergedVertices, submergedTriangles[i].a);
            submergedIndices[3 * i + 1] = Array.IndexOf(submergedVertices, submergedTriangles[i].b);
            submergedIndices[3 * i + 2] = Array.IndexOf(submergedVertices, submergedTriangles[i].c);

            submergedIndices[6 * submergedTriangles.Length - 1 - 3 * i] = Array.IndexOf(submergedVertices, submergedTriangles[submergedTriangles.Length - 1 - i].a);
            submergedIndices[6 * submergedTriangles.Length - 2 - 3 * i] = Array.IndexOf(submergedVertices, submergedTriangles[submergedTriangles.Length - 1 - i].b);
            submergedIndices[6 * submergedTriangles.Length - 3 - 3 * i] = Array.IndexOf(submergedVertices, submergedTriangles[submergedTriangles.Length - 1 - i].c);
        }

        submergedMesh.Clear();

        submergedMesh.vertices = submergedVertices;
        submergedMesh.triangles = submergedIndices;

        submergedMesh.RecalculateBounds();
        submergedMesh.RecalculateNormals();

        for(int i = 0; i < submergedIndices.Length - 1; i++)
        {
            // Debug.DrawLine(submergedVertices[submergedIndices[i]], submergedVertices[submergedIndices[i + 1]], Color.green);
        }
    }
}
