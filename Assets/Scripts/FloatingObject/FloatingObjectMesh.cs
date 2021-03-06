using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FloatingObjectMesh : MonoBehaviour
{
    public List<Mesh> availableMeshes;

    [SerializeField]private int currentMeshIndex = 0;
    public int CurrentMeshIndex
    {
        get => currentMeshIndex;
        set
        {
            currentMeshIndex = value;
            meshFilter.mesh = availableMeshes[currentMeshIndex];
            transform.position = new Vector3(0.0f, 3.0f, 0.0f);
            OnMeshChange?.Invoke(this, EventArgs.Empty);
        }
    }

    [SerializeField]private float scale = 1;
    public float Scale
    {
        get => scale;
        set 
        {
            scale = value;
            transform.localScale = new Vector3(scale, scale, scale);
            OnMeshChange?.Invoke(this, EventArgs.Empty);
        }
    }

    [SerializeField]private Material standardMaterial;
    [SerializeField]private Material wireframeMaterial;

    private bool isWireframe;
    public bool IsWireframe
    {
        get => isWireframe;
        set
        {
            isWireframe = value;
            meshRenderer.material = isWireframe ? wireframeMaterial : standardMaterial;
        }
    }

    public Vector3[] Vertices
    {
        get
        {
            Vector3[] vertices = new Vector3[meshFilter.mesh.vertexCount];

            for(int i = 0; i < meshFilter.mesh.vertexCount; i++)
            {
                vertices[i] = transform.localToWorldMatrix.MultiplyPoint3x4(meshFilter.mesh.vertices[i]);
            }
            
            return vertices;
        }
    }

    public int VertexCount { get => meshFilter.mesh.vertexCount; }
    public int[] Indices { get => meshFilter.mesh.triangles; }
    public int IndicesCount { get => meshFilter.mesh.triangles.Length; }

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    [HideInInspector]public EventHandler OnMeshChange;

    private void Awake()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>() as MeshFilter;
        meshRenderer = gameObject.GetComponent<MeshRenderer>() as MeshRenderer;

        transform.localScale = new Vector3(scale, scale, scale);
        meshFilter.mesh = availableMeshes[currentMeshIndex];
    }

    private void Start()
    {
        OnMeshChange.Invoke(this, EventArgs.Empty);
    }
}
