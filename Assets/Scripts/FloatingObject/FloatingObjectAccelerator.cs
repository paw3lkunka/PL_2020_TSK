using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FloatingObjectAccelerator : MonoBehaviour
{
    public float frontPower = 1.0f;
    public float backPower = 1.0f;
    public float turningPower = 1.0f;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>() as Rigidbody;
    }

    private void Update()
    {
        Vector3 accVec = new Vector3(0.0f, 0.0f, 0.0f);

        accVec.z += Keyboard.current.upArrowKey.isPressed ? frontPower : 0.0f;
        accVec.z -= Keyboard.current.downArrowKey.isPressed ? backPower : 0.0f;

        accVec.x += Keyboard.current.rightArrowKey.isPressed ? turningPower : 0.0f;
        accVec.x -= Keyboard.current.leftArrowKey.isPressed ? turningPower : 0.0f;

        accVec = transform.rotation * accVec;
        rigidbody.AddForceAtPosition(accVec, transform.position + rigidbody.centerOfMass);
    }
}
