using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FluidSurfaceMesh : MonoBehaviour
{
    // Fluid surface plane width and height in meters
    [SerializeField]private float size = 10.0f;
    public float Size
    {
        get => size;
        set 
        {
            size = value;
            surfaceModified = true;
        }
    }

    [HideInInspector]public float minSize = 0.0f;

    // Fluid surface plane resolution
    [SerializeField]private int resolution = 10;
    public int Resolution
    {
        get => resolution;
        set 
        {
            resolution = value;
            surfaceModified = true;
        }
    }

    private bool surfaceModified = true;

    [HideInInspector]public Vector3[] vertices;
    public Vector3[] Vertices
    {
        get
        {
            Vector3[] worldVertices = vertices;

            for(int i = 0; i < meshFilter.mesh.vertexCount; i++)
            {
                worldVertices[i] = transform.localToWorldMatrix.MultiplyPoint3x4(meshFilter.mesh.vertices[i]);
            }
            
            return worldVertices;
        }
    }

    private int[] indices;

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
        primitive.SetActive(false);
        Material diffuse = primitive.GetComponent<MeshRenderer>().sharedMaterial;
        DestroyImmediate(primitive);
        meshRenderer.material = diffuse;
    }

    private void Update()
    {
        if(Size < minSize)
        {
            Debug.Log($"Changing L from { Size } to { minSize }.");
            Size = minSize;
        }

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
        mesh.triangles = indices;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void GenerateMesh()
    {
        vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        indices = new int[(resolution * (resolution + 1) - 1) * 6];

        float offset = -(size / 2.0f);
        float tileSize = size / (float)resolution;

        for(int i = 0; i <= resolution; i++)
        {
            for(int j = 0; j <= resolution; j++)
            {
                vertices[i * (resolution + 1) + j] = new Vector3((j * tileSize) + offset, 0, (i * tileSize) + offset);
            }
        }

        for(int i = 0; i < resolution * (resolution + 1) - 1; i++)
        {
            indices[i * 6] = i;
            indices[i * 6 + 1] = i + resolution + 2;
            indices[i * 6 + 2] = i + 1;

            indices[i * 6 + 3] = i;
            indices[i * 6 + 4] = i + resolution + 1;
            indices[i * 6 + 5] = i + resolution + 2;
        }
    }
}
