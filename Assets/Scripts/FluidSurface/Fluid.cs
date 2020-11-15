using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fluid : MonoBehaviour
{
    [SerializeField]private float density;
    public float Density
    {
        get => density;
        set 
        {
            density = value;
            kinematicViscosity = dynamicViscosity / density;
        }
    }

    [SerializeField]private float dynamicViscosity;
    public float DynamicViscosity
    {
        get => dynamicViscosity;
        set
        {
            dynamicViscosity = value;
            kinematicViscosity = dynamicViscosity / density;
        }
    }

    [HideInInspector]public float kinematicViscosity;
}
