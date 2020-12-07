using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ForceVector
{
    public Vector3 beginning;
    public Vector3 offset;

    public ForceVector(Vector3 beginning, Vector3 offset)
    {
        this.beginning = beginning;
        this.offset = offset;
    }
}
