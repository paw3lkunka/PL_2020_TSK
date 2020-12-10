using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FloatingObjectAccelerator : MonoBehaviour
{
    private AccelerationInput input;
    private Vector3 accVector;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        input = new AccelerationInput();
        rigidbody = gameObject.GetComponent<Rigidbody>() as Rigidbody;
    }

    private void OnEnable()
    {
        input.Acceleration.Acceleration.performed += AccelerateObject_Performed;
        input.Acceleration.Acceleration.canceled += AccelerateObject_Canceled;
    }

    private void OnDisable()
    {
        input.Acceleration.Acceleration.performed -= AccelerateObject_Performed;
        input.Acceleration.Acceleration.canceled -= AccelerateObject_Canceled;
    }

    private void FixedUpdate()
    {

    }

    private void AccelerateObject_Performed(InputAction.CallbackContext ctx)
    {
        Vector2 accVector = ctx.ReadValue<Vector2>();
        accVector = new Vector3(accVector.x, 0.0f, accVector.y);
    }

    private void AccelerateObject_Canceled(InputAction.CallbackContext ctx)
    {
        accVector = new Vector3(0.0f, 0.0f, 0.0f);
    }
}
