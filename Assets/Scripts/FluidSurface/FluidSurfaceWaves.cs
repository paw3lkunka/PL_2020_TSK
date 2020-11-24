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
    [HideInInspector]public float currentTime;

    private void Start()
    {
        mesh = gameObject.GetComponent<FluidSurfaceMesh>() as FluidSurfaceMesh;
    }

    private void FixedUpdate()
    {
        currentTime = Time.timeSinceLevelLoad;
        for(int i = 0; i < (mesh.Resolution + 1) * (mesh.Resolution + 1); i++)
        {
            mesh.vertices[i].y = SurfaceHeightForVertex(mesh.vertices[i], currentTime);
        }
    }

    public float SurfaceHeightForVertex(Vector3 vertex, float time)
    {
        float pX = vertex.x * scale + (time * speed) + offset;
        float pZ = vertex.z * scale + (time * speed) + offset;
        
        return Mathf.PerlinNoise(pX, pZ) * height;
    }

    public void SetScale(float value)
    {
        scale = value;
    }

    public void SetSpeed(float value)
    {
        speed = value;
    }

    public void SetHeight(float value)
    {
        height = value;
    }

    public void SetOffset(float value)
    {
        offset = value;
    }
}
