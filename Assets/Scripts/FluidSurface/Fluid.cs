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
        }
    }

    [SerializeField]private float dynamicViscosity;
    public float DynamicViscosity
    {
        get => dynamicViscosity;
        set
        {
            dynamicViscosity = value;
        }
    }

    [HideInInspector]public float KinematicViscosity
    {
        get => dynamicViscosity / density;
    }
}
