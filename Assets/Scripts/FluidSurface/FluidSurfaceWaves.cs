using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FluidSurfaceMesh))]
public class FluidSurfaceWaves : MonoBehaviour
{
    public float scale = 0.7f;
    public float speed = 0.25f;
    public float height = 1.0f;
    public float offset = 0.0f;

    private FluidSurfaceMesh mesh;

    private void Start()
    {
        mesh = gameObject.GetComponent<FluidSurfaceMesh>() as FluidSurfaceMesh;
    }

    private void FixedUpdate()
    {
        for(int i = 0; i < (mesh.Resolution + 1) * (mesh.Resolution + 1); i++)
        {
            mesh.vertices[i].y = SurfaceHeightForVertex(mesh.vertices[i]);
        }
    }

    public float SurfaceHeightForVertex(Vector3 vertex)
    {
        float pX = vertex.x * scale + (Time.timeSinceLevelLoad * speed) + offset;
        float pZ = vertex.z * scale + (Time.timeSinceLevelLoad * speed) + offset;
        
        return Mathf.PerlinNoise(pX, pZ) * height;
    }
}
