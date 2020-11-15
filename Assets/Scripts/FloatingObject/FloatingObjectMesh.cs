using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FloatingObjectMesh : MonoBehaviour
{
    public List<Mesh> availableMeshes;

    private int currentMeshIndex;
    public int CurrentMeshIndex
    {
        get => currentMeshIndex;
        set
        {
            currentMeshIndex = value;
            meshFilter.mesh = availableMeshes[currentMeshIndex];
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

    private Transform transform;
    private MeshFilter meshFilter;

    private void Awake()
    {
        transform = gameObject.GetComponent<Transform>() as Transform;
        meshFilter = gameObject.GetComponent<MeshFilter>() as MeshFilter;
    }
}
