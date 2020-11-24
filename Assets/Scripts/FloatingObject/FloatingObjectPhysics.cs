using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(MeshCollider), typeof(FloatingObjectMeshParser))]
public class FloatingObjectPhysics : MonoBehaviour
{
    [SerializeField]private float mass = 100.0f;
    public float Mass
    {
        get => mass;
        set
        {
            mass = value;
            rigidbody.mass = mass;
        }
    }

    [SerializeField]private Vector3 centerOfGravity = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 CenterOfGravity
    {
        get => centerOfGravity;
        set
        {
            centerOfGravity = value;
            rigidbody.centerOfMass = centerOfGravity;
        }
    }

    public void SetCenterOfGravityX(string value)
    {
        CenterOfGravity = new Vector3(float.Parse(value), centerOfGravity.y, centerOfGravity.z);
    }

    public void SetCenterOfGravityY(string value)
    {
        CenterOfGravity = new Vector3(centerOfGravity.x, float.Parse(value), centerOfGravity.z);
    }
    
    public void SetCenterOfGravityZ(string value)
    {
        CenterOfGravity = new Vector3(centerOfGravity.x, centerOfGravity.y, float.Parse(value));
    }

    private float CfRn = 0.0f;

    public bool IsBuoyancyForce { get; set; }
    public bool IsViscousDrag { get; set; }
    public bool IsPressureDrag { get; set; }
    public bool IsSlammingForce { get; set; }

    [SerializeField]private Fluid fluid;
    
    private new Rigidbody rigidbody;
    private MeshCollider meshCollider;
    private FloatingObjectMeshParser meshParser;

    private MeshFilter meshFilter;
    private FloatingObjectMesh mesh;

    private void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>() as Rigidbody;
        meshCollider = gameObject.GetComponent<MeshCollider>() as MeshCollider;
        meshParser = gameObject.GetComponent<FloatingObjectMeshParser>() as FloatingObjectMeshParser;

        meshFilter = gameObject.GetComponent<MeshFilter>() as MeshFilter;
        mesh = gameObject.GetComponent<FloatingObjectMesh>() as FloatingObjectMesh;

        rigidbody.mass = mass;
        rigidbody.centerOfMass = centerOfGravity;
        
        IsBuoyancyForce = true;
        IsViscousDrag = true;
        IsPressureDrag = true;
        IsSlammingForce = true;
    }

    private void OnEnable()
    {
        mesh.OnMeshChange += ChangeColliderMesh_OnMeshChange;
    }
    
    private void OnDisable()
    {
        mesh.OnMeshChange -= ChangeColliderMesh_OnMeshChange;
    }

    private void FixedUpdate()
    {
        RecalculateViscousityResistanceCoefficient();

        Triangle[] submergedTriangles = meshParser.submerged.ToArray();
        for(int i = 0; i < submergedTriangles.Length; i++)
        {
            submergedTriangles[i].CalculateAdditionalValues(rigidbody.centerOfMass, rigidbody.velocity, rigidbody.angularVelocity);

            Vector3 force = new Vector3(0.0f, 0.0f, 0.0f);
            force += IsBuoyancyForce ? BuoyancyForce(submergedTriangles[i]) : new Vector3(0.0f, 0.0f, 0.0f);
            force += IsViscousDrag ? ViscousDragForce(submergedTriangles[i]) : new Vector3(0.0f, 0.0f, 0.0f);
            force += IsPressureDrag ? PressureDragForce(submergedTriangles[i]) : new Vector3(0.0f, 0.0f, 0.0f);

            rigidbody.AddForceAtPosition(force, submergedTriangles[i].center);
        }
    }

    private Vector3 BuoyancyForce(Triangle triangle)
    {
        Vector3 force = fluid.Density * Physics.gravity.magnitude * triangle.hCenter * triangle.normal * triangle.area;
        return new Vector3(0.0f, Mathf.Max(0.0f, force.y), 0.0f);
    }

    private Vector3 ViscousDragForce(Triangle triangle)
    {
        float speed = rigidbody.velocity.magnitude;

        Vector3 velocityTangent = Vector3.Cross(triangle.normal, Vector3.Cross(triangle.velocity, triangle.normal) / speed) / speed;
        Vector3 vF = -triangle.velocity.magnitude * velocityTangent.normalized;
        
        return (fluid.Density * vF * vF.magnitude * triangle.area * CfRn) / 2.0f;
    }

    private Vector3 PressureDragForce(Triangle triangle)
    {
        float speed = triangle.velocity.magnitude;
        Vector3 force = new Vector3(0.0f, 0.0f, 0.0f);

        if(triangle.cosTheta > 0.0f)
        {
            // force = -10.0f * speed * (1 + speed) * triangle.area * Mathf.Sqrt(triangle.cosTheta) * triangle.normal;
            force = -2500.0f * triangle.area * Mathf.Sqrt(triangle.cosTheta) * triangle.normal;
        }
        else
        {
            // force = 10.0f * speed * (1 + speed) * triangle.area * Mathf.Sqrt(triangle.cosTheta) * triangle.normal;
            force = 2500.0f * triangle.area * Mathf.Sqrt(Mathf.Abs(triangle.cosTheta)) * triangle.normal;
        }

        return force;
    }

    private Vector3 SlammingForce(Triangle triangle)
    {
        return new Vector3();
    }

    private void RecalculateViscousityResistanceCoefficient()
    {
        float Rn = fluid.Density * rigidbody.velocity.magnitude * fluid.kinematicViscosity;
        float temp = Mathf.Log10(Rn) - 2;
        CfRn =  0.075f / (temp * temp);
    }

    private void ChangeColliderMesh_OnMeshChange(object sender, EventArgs e)
    {
        meshCollider.sharedMesh = meshFilter.mesh;
    }
}
