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
            centerOfGravityTransform.localPosition = centerOfGravity;
        }
    }

    public void SetCenterOfGravityX(string value)
    {
        CenterOfGravity = new Vector3(Mathf.Clamp(float.Parse(value), -0.6f, 0.6f), centerOfGravity.y, centerOfGravity.z);
    }

    public void SetCenterOfGravityY(string value)
    {
        CenterOfGravity = new Vector3(centerOfGravity.x, Mathf.Clamp(float.Parse(value), 0.0f, 0.8f), centerOfGravity.z);
    }
    
    public void SetCenterOfGravityZ(string value)
    {
        CenterOfGravity = new Vector3(centerOfGravity.x, centerOfGravity.y, Mathf.Clamp(float.Parse(value), -1.5f, 1.5f));
    }

    public Transform centerOfGravityTransform;

    private float CfRn = 0.0f;
    private float GammaMax = 0.0f;

    public float linearPressureCoefficient = 10.0f;
    public float quadraticPressureCoefficient = 10.0f;
    public float pressureFalloffPower = 0.5f;

    public float linearSuctionCoefficient = 10.0f;
    public float quadraticSuctionCoefficient = 10.0f;
    public float suctionFalloffPower = 0.5f;

    public float slammingPower = 2.0f;

    public bool IsBuoyancyForce { get; set; }
    public bool IsViscousDrag { get; set; }
    public bool IsPressureDrag { get; set; }
    public bool IsSlammingForce { get; set; }

    public bool isStopped = false;
    public bool IsStopped
    {
        get => isStopped;
        set
        {
            isStopped = value;
            rigidbody.isKinematic = isStopped;
        }
    }

    [SerializeField]private Fluid fluid;
    
    private new Rigidbody rigidbody;
    private MeshCollider meshCollider;
    private FloatingObjectMeshParser meshParser;

    private MeshFilter meshFilter;
    private FloatingObjectMesh mesh;

    [HideInInspector]public List<ForceVector> buoyancyForceVectors;
    [HideInInspector]public List<ForceVector> viscosityDragVectors;
    [HideInInspector]public List<ForceVector> pressureDragVectors;
    [HideInInspector]public List<ForceVector> slammingForceVectors;

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

        buoyancyForceVectors = new List<ForceVector>();
        viscosityDragVectors = new List<ForceVector>();
        pressureDragVectors = new List<ForceVector>();
        slammingForceVectors = new List<ForceVector>();

        IsStopped = false;
    }

    private void Start()
    {
        centerOfGravityTransform.localPosition = centerOfGravity;
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
        buoyancyForceVectors.Clear();
        viscosityDragVectors.Clear();
        pressureDragVectors.Clear();
        slammingForceVectors.Clear();
        RecalculateViscousityResistanceCoefficient();

        Triangle[] submergedTriangles = meshParser.submerged.ToArray();
        for(int i = 0; i < submergedTriangles.Length; i++)
        {
            submergedTriangles[i].CalculateAdditionalValues(rigidbody.centerOfMass, rigidbody.velocity, rigidbody.angularVelocity);

            Vector3 force = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 forceBeginning = submergedTriangles[i].center;
            
            if(IsBuoyancyForce)
            {
                Vector3 buoyancy = BuoyancyForce(submergedTriangles[i]);
                force += buoyancy;
                buoyancyForceVectors.Add(new ForceVector(forceBeginning, buoyancy));
            }

            if(IsViscousDrag)
            {
                Vector3 viscousDrag = ViscousDragForce(submergedTriangles[i]);
                force += viscousDrag;
                buoyancyForceVectors.Add(new ForceVector(forceBeginning, viscousDrag));
            }

            if(IsPressureDrag)
            {
                Vector3 pressureDrag = PressureDragForce(submergedTriangles[i]);
                force += pressureDrag;
                buoyancyForceVectors.Add(new ForceVector(forceBeginning, pressureDrag));
            }

            //force = force.magnitude < 10000.0f ? force : new Vector3(0.0f, 0.0f, 0.0f);

            rigidbody.AddForceAtPosition(force, forceBeginning);
        }

        if(IsSlammingForce)
        {
            Polygon[] polygons = meshParser.polygons;
            for(int i = 0; i < polygons.Length; i++)
            {
                polygons[i].CalculateAdditionalValues(rigidbody.centerOfMass, rigidbody.velocity, rigidbody.angularVelocity);
                GammaMax = polygons[i].gamma.magnitude > GammaMax ? polygons[i].gamma.magnitude : GammaMax;
            }

            for(int i = 0; i < polygons.Length; i++)
            {
                Vector3 slamming = SlammingForce(polygons[i]);
                slammingForceVectors.Add(new ForceVector(polygons[i].center, slamming));
                rigidbody.AddForceAtPosition(slamming, polygons[i].center);
            }
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
        
        return -(fluid.Density * vF * vF.magnitude * triangle.area * CfRn) / 2.0f;
    }

    private Vector3 PressureDragForce(Triangle triangle)
    {
        float speed = triangle.velocity.magnitude;
        // float speed = 1.0f;
        Vector3 force = new Vector3(0.0f, 0.0f, 0.0f);

        if(triangle.cosTheta > 0.0f)
        {
            // force = -speed * (linearPressureCoefficient + quadraticPressureCoefficient * speed) * triangle.area * Mathf.Pow(triangle.cosTheta, pressureFalloffPower) * triangle.normal;
            force = -(linearPressureCoefficient * speed + quadraticPressureCoefficient * speed * speed) * triangle.area * Mathf.Pow(triangle.cosTheta, pressureFalloffPower) * triangle.normal;
        }
        else
        {
            // force = speed * (linearSuctionCoefficient + quadraticSuctionCoefficient * speed) * triangle.area * Mathf.Pow(triangle.cosTheta, suctionFalloffPower) * triangle.normal;
            force = (linearSuctionCoefficient * speed + quadraticSuctionCoefficient * speed * speed) * triangle.area * Mathf.Pow(Mathf.Abs(triangle.cosTheta), pressureFalloffPower) * triangle.normal;
        }

        return force;
    }

    private Vector3 SlammingForce(Polygon polygon)
    {
        Vector3 stopForce = rigidbody.mass * polygon.velocity * (2.0f * polygon.submergedArea) / meshParser.totalArea;
        if (!float.IsNaN(transform.position.x) && !float.IsNaN(transform.position.y) && !float.IsNaN(transform.position.z))
        {
            return Mathf.Pow(Mathf.Clamp(polygon.gamma.magnitude / GammaMax, 0.0f, 1.0f), slammingPower) * polygon.cosTheta * stopForce;
        }
        else
        {
            return new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    private void RecalculateViscousityResistanceCoefficient()
    {
        float Rn = fluid.Density * rigidbody.velocity.magnitude * fluid.KinematicViscosity;
        float temp = Mathf.Log10(Rn) - 2;
        CfRn =  0.075f / (temp * temp);
    }

    private void ChangeColliderMesh_OnMeshChange(object sender, EventArgs e)
    {
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(0.0f, 6.0f, 0.0f);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    }
}
