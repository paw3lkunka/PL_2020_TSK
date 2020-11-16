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

    private float CfRn = 0.0f;

    [SerializeField]private Fluid fluid;
    
    private Rigidbody rigidbody;
    private FloatingObjectMeshParser meshParser;

    private void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>() as Rigidbody;
        meshParser = gameObject.GetComponent<FloatingObjectMeshParser>() as FloatingObjectMeshParser;

        rigidbody.mass = mass;
        rigidbody.centerOfMass = centerOfGravity;
    }

    private void FixedUpdate()
    {
        float CfRn = ViscousResistanceCoefficient();

        for(int i = 0; i < meshParser.submerged.Count; i++)
        {
            rigidbody.AddForceAtPosition(BuoyancyForce(meshParser.submerged[i]), meshParser.submerged[i].center);
        }
    }

    private Vector3 BuoyancyForce(Triangle triangle)
    {
        return fluid.Density * Physics.gravity.magnitude * triangle.hCenter * triangle.normal * triangle.area;
    }

    private Vector3 ViscousDragForce(Triangle triangle)
    {
        float speed = rigidbody.velocity.magnitude;
        return new Vector3() * (fluid.Density * speed * speed * triangle.area * CfRn) / 2.0f;
    }

    private Vector3 PressureDragForce(Triangle triangle)
    {
        return new Vector3();
    }

    private Vector3 SlammingForce(Triangle triangle)
    {
        return new Vector3();
    }

    private float ViscousResistanceCoefficient()
    {
        float Rn = fluid.Density * rigidbody.velocity.magnitude * fluid.kinematicViscosity;
        float temp = Mathf.Log10(Rn) - 2;
        return 0.075f / (temp * temp);
    }
}
