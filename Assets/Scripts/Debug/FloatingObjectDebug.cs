using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FloatingObjectDebug : MonoBehaviour
{
    [SerializeField]private GameObject floatingObject;  
    [SerializeField]private Color buoyancyColor;  
    [SerializeField]private Color viscosityColor;  
    [SerializeField]private Color pressureColor;  
    [SerializeField]private Color slammingColor;  
    [SerializeField]private Material material;
    private FloatingObjectMeshParser meshParser;
    private FloatingObjectPhysics physics;

    [SerializeField]private float forcesLength = 0.01f;
    
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

    private void OnPostRender()
    {
        DrawForces();
    }
    
    private void OnDrawGizmos()
    {
        GL.Begin(GL.LINES);
        material.SetPass(0);
        GL.Color(Color.red);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(2, 2, 2);
        GL.End();
        DrawForces();
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
    }

    private void DrawForces()
    {
        GL.Begin(GL.LINES);
        material.SetPass(0);
        
        foreach(ForceVector vector in physics.buoyancyForceVectors)
        {
            DrawForceVector(vector, buoyancyColor);
        }

        foreach(ForceVector vector in physics.viscosityDragVectors)
        {
            DrawForceVector(vector, viscosityColor);
        }

        foreach(ForceVector vector in physics.pressureDragVectors)
        {
            DrawForceVector(vector, pressureColor);
        }

        foreach(ForceVector vector in physics.slammingForceVectors)
        {
            DrawForceVector(vector, slammingColor);
        }

        GL.End();
    }

    private void DrawForceVector(ForceVector vector, Color color)
    {
        GL.Color(color);
        GL.Vertex(vector.beginning);
        GL.Vertex(vector.beginning + vector.offset * forcesLength);
    }
}
