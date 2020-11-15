using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FluidSurfaceMesh : MonoBehaviour
{
    // Fluid surface plane width and height in meters
    [SerializeField]private float l = 10;
    public float L
    {
        get => l;
        set 
        {
            l = value;
            surfaceModified = true;
        }
    }

    // Fluid surface plane resolution
    [SerializeField]private int n = 10;
    public int N
    {
        get => n;
        set 
        {
            n = value;
            surfaceModified = true;
        }
    }

    private bool surfaceModified = true;

    public Vector3[] vertices;
    private int[] indicies;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>() as MeshFilter;
        meshRenderer = gameObject.GetComponent<MeshRenderer>() as MeshRenderer;
        
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
        primitive.active = false;
        Material diffuse = primitive.GetComponent<MeshRenderer>().sharedMaterial;
        DestroyImmediate(primitive);
        meshRenderer.material = diffuse;
    }

    private void Update()
    {
        if(surfaceModified)
        {
            GenerateMesh();
            surfaceModified = false;
        }
    }

    private void FixedUpdate()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = indicies;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void GenerateMesh()
    {
        vertices = new Vector3[(n + 1) * (n + 1)];
        indicies = new int[(n * (n + 1) - 1) * 6];

        float offset = -(l / 2.0f);
        float tileSize = l / (float)n;

        for(int i = 0; i <= n; i++)
        {
            for(int j = 0; j <= n; j++)
            {
                vertices[i * (n + 1) + j] = new Vector3((j * tileSize) + offset, 0, (i * tileSize) + offset);
            }
        }

        for(int i = 0; i < n * (n + 1) - 1; i++)
        {
            indicies[i * 6] = i;
            indicies[i * 6 + 1] = i + n + 2;
            indicies[i * 6 + 2] = i + 1;

            indicies[i * 6 + 3] = i;
            indicies[i * 6 + 4] = i + n + 1;
            indicies[i * 6 + 5] = i + n + 2;
        }
    }
}
