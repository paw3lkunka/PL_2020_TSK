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
        for(int i = 0; i < (mesh.N + 1) * (mesh.N + 1); i++)
        {
            float pX = (mesh.vertices[i].x + transform.position.x) * scale + (Time.timeSinceLevelLoad * speed) + offset;
            float pZ = (mesh.vertices[i].z + transform.position.z) * scale + (Time.timeSinceLevelLoad * speed) + offset;
            
            mesh.vertices[i].y = Mathf.PerlinNoise(pX, pZ) * height;
        }
    }
}
