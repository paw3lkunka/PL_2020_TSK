using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon : Triangle
{
    public float lastSubmergedArea;
    public float submergedArea;

    public Vector3 lastVelocity;
    public Vector3 gamma;

    public Polygon(Vector3 a, Vector3 b, Vector3 c) 
        : base(a, b, c)
    {}

    public Polygon()
        : this(new Vector3(), new Vector3(), new Vector3())
    {}

    public void UpdateVertices(Vector3 a, Vector3 b, Vector3 c)
    {
        lastSubmergedArea = submergedArea;
        submergedArea = 0.0f;

        this.a = a;
        this.b = b;
        this.c = c;

        center = GetCenter(a, b, c);
        normal = GetNormal(a, b, c);
        area = GetArea(a, b, c);

        velocity = new Vector3();
        normalizedVelocity = new Vector3();
        cosTheta = 0.0f;
    }

    public override void CalculateAdditionalValues(Vector3 objectCenterOfGravity, Vector3 objectVelocity, Vector3 objectAngularVelocity)
    {
        lastVelocity = velocity;
        base.CalculateAdditionalValues(objectCenterOfGravity, objectVelocity, objectAngularVelocity);

        gamma = (submergedArea * velocity - lastSubmergedArea * lastVelocity) / area;
    }
}
