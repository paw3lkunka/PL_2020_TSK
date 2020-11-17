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
        
        // GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
        // primitive.SetActive(false);
        // Material diffuse = primitive.GetComponent<MeshRenderer>().sharedMaterial;
        // DestroyImmediate(primitive);
        // meshRenderer.material = diffuse;
    }

    private void Update()
    {
        GenerateSubmergedMesh();
    }

    private void GenerateSubmergedMesh()
    {
        // Vector3[] submergedVertices = new Vector3[meshParser.submerged.Count * 3];
        // int[] submergedIndices = new int[meshParser.submerged.Count * 3];
        HashSet<Vector3> submergedVerticesSet = new HashSet<Vector3>();

        // submergedVertices.AddRange(new Vector3[] {meshParser.submerged[0].a, meshParser.submerged[0].b, meshParser.submerged[0].c});
        // submergedIndices.AddRange(new int[] {0, 1, 2});

        for(int i = 0; i < meshParser.submerged.Count; i++)
        {
            submergedVerticesSet.UnionWith(new Vector3[] {meshParser.submerged[i].a, meshParser.submerged[i].b, meshParser.submerged[i].c});
        }

        Vector3[] submergedVertices = submergedVerticesSet.ToArray();
        int[] submergedIndices = new int[meshParser.submerged.Count * 3];

        for(int i = 0; i < meshParser.submerged.Count; i++)
        {
            submergedIndices[3 * i] = Array.IndexOf(submergedVertices, meshParser.submerged[i].a);
            submergedIndices[3 * i + 1] = Array.IndexOf(submergedVertices, meshParser.submerged[i].b);
            submergedIndices[3 * i + 2] = Array.IndexOf(submergedVertices, meshParser.submerged[i].c);

            // submergedIndices[3 * i + 5] = Array.IndexOf(submergedVertices, meshParser.submerged[i].a);
            // submergedIndices[3 * i + 4] = Array.IndexOf(submergedVertices, meshParser.submerged[i].b);
            // submergedIndices[3 * i + 3] = Array.IndexOf(submergedVertices, meshParser.submerged[i].c);
        }

        // meshParser.submerged.Count
        // for(int i = 0; i < meshParser.submerged.Count; i++)
        // {
            // submergedVertices[i * 3] = meshParser.submerged[i].a;
            // submergedVertices[i * 3 + 1] = meshParser.submerged[i].b;
            // submergedVertices[i * 3 + 2] = meshParser.submerged[i].c;
        // }

        // for(int i = 0; i < meshParser.submerged.Count * 3; i++)
        // {
            // submergedIndices[i] = i;
        // }
        
        submergedMesh.Clear();

        // submergedMesh.vertices = submergedVertices;
        // submergedMesh.triangles = submergedIndices;
        submergedMesh.vertices = submergedVerticesSet.ToArray();
        submergedMesh.triangles = submergedIndices.ToArray();

        submergedMesh.RecalculateBounds();
        submergedMesh.RecalculateNormals();
    }
}
