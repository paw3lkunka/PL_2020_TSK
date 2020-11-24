using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidSurfaceNavigator : MonoBehaviour
{
    [SerializeField]private GameObject floatingObject;
    private Transform objectTransform;
    private FloatingObjectMesh objectMesh;
    
    private FluidSurfaceMesh mesh;

    private void Awake()
    {
        objectTransform = floatingObject.GetComponent<Transform>() as Transform;
        objectMesh = floatingObject.GetComponent<FloatingObjectMesh>() as FloatingObjectMesh;
        
        mesh = GetComponent<FluidSurfaceMesh>() as FluidSurfaceMesh;
    }

    private void OnEnable()
    {
        objectMesh.OnMeshChange += SurfaceSizeControl_OnMeshChange;
    }
    
    private void OnDisable()
    {
        objectMesh.OnMeshChange -= SurfaceSizeControl_OnMeshChange;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(objectTransform.position.x, 0.0f, objectTransform.position.z);
    }

    private void SurfaceSizeControl_OnMeshChange(object sender, EventArgs e)
    {
        float maxDistance = 0.0f;

        for(int i = 0; i < objectMesh.VertexCount; i++)
        {
            float currentDistance = (objectMesh.Vertices[i] - objectTransform.position).sqrMagnitude;
            if(maxDistance < currentDistance)
            {
                maxDistance = currentDistance;
            }
        }
        
        mesh.minSize = maxDistance * 2.5f;
    }
}
