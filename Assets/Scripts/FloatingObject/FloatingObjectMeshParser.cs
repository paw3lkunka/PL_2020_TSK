using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(FloatingObjectMesh))]
public class FloatingObjectMeshParser : MonoBehaviour
{
    [HideInInspector]public List<Triangle> submerged;
    [HideInInspector]public List<Triangle> surfaced;
    
    [HideInInspector]public float totalArea;
    [HideInInspector]public Polygon[] polygons;

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

    private void Start()
    {
        polygons = new Polygon[mesh.IndicesCount / 3];
        for(int i = 0; i < mesh.IndicesCount / 3; i++)
        {
            polygons[i] = new Polygon();
        }
    }

    private void OnEnable()
    {
        mesh.OnMeshChange += UpdatePolygons_OnMeshChange;
    }

    private void OnDisable()
    {
        mesh.OnMeshChange -= UpdatePolygons_OnMeshChange;
    }

    private void FixedUpdate()
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
            verticesHeights[i] = vertices[i].y - surfaceWaves.SurfaceHeightForVertex(vertices[i], surfaceWaves.currentTime);
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
        totalArea = 0.0f;

        for(int i = 0; i < mesh.IndicesCount; i += 3)
        {
            Tuple<int, Vector3, float>[] tr = new Tuple<int, Vector3, float>[3];
            tr[0] = new Tuple<int, Vector3, float>(0, vertices[indices[i]], verticesHeights[indices[i]]);
            tr[1] = new Tuple<int, Vector3, float>(1, vertices[indices[i + 1]], verticesHeights[indices[i + 1]]);
            tr[2] = new Tuple<int, Vector3, float>(2, vertices[indices[i + 2]], verticesHeights[indices[i + 2]]);

            tr = tr.OrderByDescending(o => o.Item3).ToArray();
            
            Vector3 h = tr[0].Item2;
            Vector3 m = tr[1].Item2;
            Vector3 l = tr[2].Item2;

            polygons[i / 3].UpdateVertices(h, m, l);
            totalArea += polygons[i / 3].area;

            float hH = tr[0].Item3;
            float hM = tr[1].Item3;
            float hL = tr[2].Item3;
            
            if(hH <= 0.0f)
            {
                // tr = tr.OrderBy(o => o.Item1).ToArray();
                Triangle submergedTriangle = new Triangle(tr[0].Item2, tr[1].Item2, tr[2].Item2, tr[0].Item3, tr[1].Item3, tr[2].Item3);
                submerged.Add(submergedTriangle);
                polygons[i / 3].submergedArea = submergedTriangle.area;
            }
            else if(hM <= 0.0f)
            {
                float tM = -hM / (hH - hM);
                float tL = -hL / (hH - hL);
                Vector3 iM = m + (h - m) * tM;
                Vector3 iL = l + (h - l) * tL;
                // float hIM = CalculateVertexHeight(iM, surfaceVertices);
                // float hIL = CalculateVertexHeight(iL, surfaceVertices);
                float hIM = iM.y - surfaceWaves.SurfaceHeightForVertex(iM, surfaceWaves.currentTime);
                float hIL = iL.y - surfaceWaves.SurfaceHeightForVertex(iL, surfaceWaves.currentTime);

                Tuple<int, Vector3, float>[] subTr = new Tuple<int, Vector3, float>[3];

                Triangle firstSubmergedTriangle = new Triangle(l, m, iM, hL, hM, hIM);
                submerged.Add(firstSubmergedTriangle);
                // subTr[0] = new Tuple<int, Vector3, float>(tr[1].Item1, m, hM);
                // subTr[1] = new Tuple<int, Vector3, float>(tr[2].Item1, l, hL);
                // subTr[2] = new Tuple<int, Vector3, float>(tr[0].Item1, iM, hIM);
                // subTr = subTr.OrderBy(o => o.Item1).ToArray();
                // submerged.Add(new Triangle(subTr[0].Item2, subTr[1].Item2, subTr[2].Item2, subTr[0].Item3, subTr[1].Item3, subTr[2].Item3));

                Triangle secondSubmergedTriangle = new Triangle(l, iL, iM, hL, hIL, hIM);
                submerged.Add(secondSubmergedTriangle);
                // subTr[0] = new Tuple<int, Vector3, float>(tr[2].Item1, l, hL);
                // subTr[1] = new Tuple<int, Vector3, float>(tr[2].Item1, iL, hIL);
                // subTr[2] = new Tuple<int, Vector3, float>(tr[1].Item1, iM, hIM);
                // subTr = subTr.OrderBy(o => o.Item1).ToArray();
                // submerged.Add(new Triangle(subTr[0].Item2, subTr[1].Item2, subTr[2].Item2, subTr[0].Item3, subTr[1].Item3, subTr[2].Item3));

                // surfaced.Add(new Triangle(h, iM, iL, hH, hIM, hIL));
                // subTr[0] = new Tuple<int, Vector3, float>(tr[0].Item1, h, hH);
                // subTr[1] = new Tuple<int, Vector3, float>(tr[1].Item1, iM, hIM);
                // subTr[2] = new Tuple<int, Vector3, float>(tr[2].Item1, iL, hIL);
                // subTr = subTr.OrderBy(o => o.Item1).ToArray();
                // surfaced.Add(new Triangle(subTr[0].Item2, subTr[1].Item2, subTr[2].Item2, subTr[0].Item3, subTr[1].Item3, subTr[2].Item3));
                
                polygons[i / 3].submergedArea += firstSubmergedTriangle.area;
                polygons[i / 3].submergedArea += secondSubmergedTriangle.area;
            }
            else if(hL <= 0.0f)
            {
                float tM = -hL / (hM - hL);
                float tH = -hL / (hH - hL);
                Vector3 jM = l + (m - l) * tM;
                Vector3 jH = l + (h -l) * tH;
                // float hJM = CalculateVertexHeight(jM, surfaceVertices);
                // float hJH = CalculateVertexHeight(jH, surfaceVertices);
                float hJM = jM.y - surfaceWaves.SurfaceHeightForVertex(jM, surfaceWaves.currentTime);
                float hJH = jH.y - surfaceWaves.SurfaceHeightForVertex(jH, surfaceWaves.currentTime); 

                Tuple<int, Vector3, float>[] subTr = new Tuple<int, Vector3, float>[3];

                Triangle submergedTriangle = new Triangle(l, jM, jH, hL, hJM, hJH);
                submerged.Add(submergedTriangle);
                // subTr[0] = new Tuple<int, Vector3, float>(tr[2].Item1, l, hL);
                // subTr[1] = new Tuple<int, Vector3, float>(tr[1].Item1, jM, hJM);
                // subTr[2] = new Tuple<int, Vector3, float>(tr[0].Item1, jH, hJH);
                // subTr = subTr.OrderBy(o => o.Item1).ToArray();
                // submerged.Add(new Triangle(subTr[0].Item2, subTr[1].Item2, subTr[2].Item2, subTr[0].Item3, subTr[1].Item3, subTr[2].Item3));

                // surfaced.Add(new Triangle(h, jH, m, hH, hJH, hM));
                // subTr[0] = new Tuple<int, Vector3, float>(tr[0].Item1, h, hH);
                // subTr[1] = new Tuple<int, Vector3, float>(tr[1].Item1, m, hM);
                // subTr[2] = new Tuple<int, Vector3, float>(tr[0].Item1, jH, hJH);
                // subTr = subTr.OrderBy(o => o.Item1).ToArray();
                // surfaced.Add(new Triangle(subTr[0].Item2, subTr[1].Item2, subTr[2].Item2, subTr[0].Item3, subTr[1].Item3, subTr[2].Item3));

                // surfaced.Add(new Triangle(h, m, jM, hH, hM, hJM));
                // subTr[0] = new Tuple<int, Vector3, float>(tr[0].Item1, h, hH);
                // subTr[1] = new Tuple<int, Vector3, float>(tr[1].Item1, m, hM);
                // subTr[2] = new Tuple<int, Vector3, float>(tr[1].Item1, jM, hJM);
                // subTr = subTr.OrderBy(o => o.Item1).ToArray();
                // surfaced.Add(new Triangle(subTr[0].Item2, subTr[1].Item2, subTr[2].Item2, subTr[0].Item3, subTr[1].Item3, subTr[2].Item3));

                polygons[i / 3].submergedArea += submergedTriangle.area;
            }
            else
            {
                // tr = tr.OrderBy(o => o.Item1).ToArray();
                // surfaced.Add(new Triangle(tr[0].Item2, tr[1].Item2, tr[2].Item2, tr[0].Item3, tr[1].Item3, tr[2].Item3));
            }
        }
    }

    public void UpdatePolygons_OnMeshChange(object sender, EventArgs e)
    {
        polygons = new Polygon[mesh.IndicesCount / 3];
        for(int i = 0; i < mesh.IndicesCount / 3; i++)
        {
            polygons[i] = new Polygon();
        }
    }
}
