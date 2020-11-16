using System.Collections;
using System.Collections.Generic;
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
        
        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
        primitive.SetActive(false);
        Material diffuse = primitive.GetComponent<MeshRenderer>().sharedMaterial;
        DestroyImmediate(primitive);
        meshRenderer.material = diffuse;
    }

    private void FixedUpdate()
    {
        GenerateSubmergedMesh();
    }

    private void GenerateSubmergedMesh()
    {
        Vector3[] submergedVertices = new Vector3[meshParser.submerged.Count * 3];
        int[] submergedIndices = new int[meshParser.submerged.Count * 3];
        
        // meshParser.submerged.Count
        for(int i = 0; i < meshParser.submerged.Count; i++)
        {
            submergedVertices[i * 3] = meshParser.submerged[i].a;
            submergedVertices[i * 3 + 1] = meshParser.submerged[i].b;
            submergedVertices[i * 3 + 2] = meshParser.submerged[i].c;
        }

        for(int i = 0; i < meshParser.submerged.Count * 3; i++)
        {
            submergedIndices[i] = i;
        }
        
        submergedMesh.Clear();

        submergedMesh.vertices = submergedVertices;
        submergedMesh.triangles = submergedIndices;

        submergedMesh.RecalculateBounds();
        submergedMesh.RecalculateNormals();
    }
}
