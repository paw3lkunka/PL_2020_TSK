using UnityEngine;

public struct Triangle
{
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;

    public Vector3 center;
    public Vector3 normal;
    public float area;

    public float hA;
    public float hB;
    public float hC;
    public float hCenter;

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;

        center = GetCenter(a, b, c);
        normal = GetNormal(a, b, c);
        area = GetArea(a, b, c);

        hA = 0.0f;
        hB = 0.0f;
        hC = 0.0f;
        hCenter = 0.0f;
    }

    public Triangle(Vector3 a, Vector3 b, Vector3 c, float hA, float hB, float hC)
        : this(a,b,c)
    {
        this.hA = hA;
        this.hB = hB;
        this.hC = hC;
        hCenter = (hA + hB + hC) / 3.0f;
    }

    public static Vector3 GetCenter(Vector3 a, Vector3 b, Vector3 c)
    {
        return (a + b + c) / 3.0f;
    }

    public static Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        return Vector3.Cross(b - a, c - a).normalized;
    }

    public static float GetArea(Vector3 a, Vector3 b, Vector3 c)
    {
        return Vector3.Cross(c - a, b - a).magnitude / 2.0f;
    }

    public static Vector3 GetCenterVelocity(Vector3 triangleCenter, Vector3 objectCenterOfGravity, Vector3 objectVelocity, Vector3 objectAngularVelocity)
    {
        return objectVelocity + Vector3.Cross(objectAngularVelocity, triangleCenter - objectCenterOfGravity);
    }
}
