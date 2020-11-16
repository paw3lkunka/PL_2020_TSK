﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(FloatingObjectMesh))]
public class FloatingObjectMeshParser : MonoBehaviour
{
    [HideInInspector]public List<Triangle> submerged;
    [HideInInspector]public List<Triangle> surfaced;

    private float[] verticesHeights;

    private FloatingObjectMesh mesh;
    [SerializeField]private FluidSurfaceMesh surfaceMesh;
    private FluidSurfaceWaves surfaceWaves;

    private void Awake()
    {
        submerged = new List<Triangle>();
        surfaced = new List<Triangle>();

        mesh = gameObject.GetComponent<FloatingObjectMesh>() as FloatingObjectMesh;
        surfaceWaves = surfaceMesh.gameObject.GetComponent<FluidSurfaceWaves>() as FluidSurfaceWaves;
    }
    
    private void Update()
    {
        Vector3[] vertices = mesh.Vertices;
        int[] indices = mesh.Indices;
        Vector3[] surfaceVertices = surfaceMesh.Vertices;
        
        RecalculateVerticesHeights(vertices, surfaceVertices);
        RecalculateSubmergedTriangles(vertices, indices, surfaceVertices);
    }

    private void RecalculateVerticesHeights(Vector3[] vertices, Vector3[] surfaceVertices)
    {
        verticesHeights = new float[mesh.VertexCount];

        for(int i = 0; i < mesh.VertexCount; i++)
        {
            // verticesHeights[i] = CalculateVertexHeight(vertices[i], surfaceVertices);
            verticesHeights[i] = surfaceWaves.SurfaceHeightForVertex(vertices[i]);
        }
    }

    private float CalculateVertexHeight(Vector3 vertex, Vector3[]  surfaceVertices)
    {
            int i = Mathf.FloorToInt((vertex.x - surfaceVertices[0].x) / (float)surfaceMesh.Resolution);
            int j = Mathf.FloorToInt((vertex.z - surfaceVertices[0].z) / (float)surfaceMesh.Resolution);

            float innerX = (vertex.x - surfaceVertices[0].x) - (float)i * (surfaceMesh.Size / surfaceMesh.Resolution);
            float innerZ = (vertex.z - surfaceVertices[0].z) - (float)j * (surfaceMesh.Size / surfaceMesh.Resolution);

            Vector3 a, b, c;

            if(innerX >= innerZ)
            {
                a = surfaceVertices[i * (surfaceMesh.Resolution + 1) + j];
                b = surfaceVertices[(i + 1) * (surfaceMesh.Resolution + 1) + (j + 1)];
                c = surfaceVertices[i * (surfaceMesh.Resolution + 1) + (j + 1)];
            }
            else
            {
                a = surfaceVertices[i * (surfaceMesh.Resolution + 1) + j];
                b = surfaceVertices[(i + 1) * (surfaceMesh.Resolution + 1) + (j + 1)];
                c = surfaceVertices[(i + 1) * (surfaceMesh.Resolution + 1) + j];
            }

            Vector3 abcVec = Vector3.Cross(b - a, c - a);
            float d = Vector3.Dot(abcVec, a);

            return vertex.y - (d - abcVec.x * vertex.x - abcVec.z * vertex.z) / abcVec.y;
    }

    private void RecalculateSubmergedTriangles(Vector3[] vertices, int[] indices, Vector3[] surfaceVertices)
    {
        submerged.Clear();
        surfaced.Clear();

        for(int i = 0; i < mesh.IndicesCount; i += 3)
        {
            KeyValuePair<Vector3, float>[] meshTriangle = new KeyValuePair<Vector3, float>[3];
            meshTriangle[0] = new KeyValuePair<Vector3, float>(vertices[indices[i]], verticesHeights[indices[i]]);
            meshTriangle[1] = new KeyValuePair<Vector3, float>(vertices[indices[i + 1]], verticesHeights[indices[i + 1]]);
            meshTriangle[2] = new KeyValuePair<Vector3, float>(vertices[indices[i + 2]], verticesHeights[indices[i + 2]]);

            // meshTriangle.OrderByDescending(o => o.Value);
            
            Vector3 h = meshTriangle[0].Key;
            Vector3 m = meshTriangle[1].Key;
            Vector3 l = meshTriangle[2].Key;

            float hH = meshTriangle[0].Value;
            float hM = meshTriangle[1].Value;
            float hL = meshTriangle[2].Value;

            if(hH <= 0.0f)
            {
                submerged.Add(new Triangle(h, m, l, hH, hM, hL));
            }
            else if(hM <= 0.0f)
            {
                float tM = -hM / (hH - hM);
                float tL = -hL / (hH - hL);
                Vector3 iM = m + (h - m) * tM;
                Vector3 iL = l + (h - l) * tL;
                // float hIM = CalculateVertexHeight(iM, surfaceVertices);
                // float hIL = CalculateVertexHeight(iL, surfaceVertices);
                float hIM = surfaceWaves.SurfaceHeightForVertex(iM);
                float hIL = surfaceWaves.SurfaceHeightForVertex(iL);

                submerged.Add(new Triangle(m, iM, l, hM, hIM, hL));
                submerged.Add(new Triangle(l, iL, iM, hL, hIL, hIM));
                // surfaced.Add(new Triangle(h, iM, iL, hH, hIM, hIL));
            }
            else if(hL <= 0.0f)
            {
                float tM = -hL / (hM - hL);
                float tH = -hL / (hH - hL);
                Vector3 jM = l + (m - l) * tM;
                Vector3 jH = l + (h -l) * tH;
                // float hJM = CalculateVertexHeight(jM, surfaceVertices);
                // float hJH = CalculateVertexHeight(jH, surfaceVertices);
                float hJM = surfaceWaves.SurfaceHeightForVertex(jM);
                float hJH = surfaceWaves.SurfaceHeightForVertex(jH); 
                
                submerged.Add(new Triangle(l, jM, jH, hL, hJM, hJH));
                // surfaced.Add(new Triangle(h, jH, m, hH, hJH, hM));
                // surfaced.Add(new Triangle(h, m, jM, hH, hM, hJM));
            }
            else
            {
                // surfaced.Add(new Triangle(h, m, l, hH, hM, hL));
            }
        }
    }
}