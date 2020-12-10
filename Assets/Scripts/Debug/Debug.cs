using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Debug : MonoBehaviour
{
    [SerializeField]private GameObject floatingObject;  
    [SerializeField]private GameObject submergedDebug;

    [SerializeField]private Material material;
    [SerializeField]private float forcesLength = 0.01f;

    [SerializeField]private Color buoyancyColor;  
    [SerializeField]private Color viscosityColor;  
    [SerializeField]private Color pressureColor;  
    [SerializeField]private Color slammingColor;  
    
    private bool isSubmergedSurface;
    public bool IsSubmergedSurface 
    { 
        get => isSubmergedSurface ;
        set
        {
            isSubmergedSurface = value;
            meshRenderer.enabled = value;
        }
    }
    public bool IsForcesVectors { get; set; }
    public bool IsBodyAcceleration { get; set; }

    private FloatingObjectMeshParser meshParser;
    private FloatingObjectPhysics physics;
    
    private Mesh submergedMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshFilter = submergedDebug.GetComponent<MeshFilter>() as MeshFilter;
        meshRenderer = submergedDebug.GetComponent<MeshRenderer>() as MeshRenderer;

        meshParser = floatingObject.GetComponent<FloatingObjectMeshParser>() as FloatingObjectMeshParser;
        physics = floatingObject.GetComponent<FloatingObjectPhysics>() as FloatingObjectPhysics;

        submergedMesh = new Mesh();
        meshFilter.mesh = submergedMesh;
    }

    private void FixedUpdate()
    {
        if(IsSubmergedSurface)
        {
            GenerateSubmergedMesh();
        }
    }

    private void OnPostRender()
    {
        if(IsForcesVectors)
        {
            DrawForces();
        }
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
            GL.Color(buoyancyColor);
            DrawForceVector(vector);
        }

        foreach(ForceVector vector in physics.viscosityDragVectors)
        {
            GL.Color(viscosityColor);
            DrawForceVector(vector);
        }

        foreach(ForceVector vector in physics.pressureDragVectors)
        {
            GL.Color(pressureColor);
            DrawForceVector(vector);
        }

        foreach(ForceVector vector in physics.slammingForceVectors)
        {
            GL.Color(slammingColor);
            DrawForceVector(vector);
        }

        GL.End();
    }

    private void DrawForceVector(ForceVector vector)
    {
        GL.Vertex(vector.beginning);
        GL.Vertex(vector.beginning + vector.offset * forcesLength);
    }

    public void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
