using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private float pushForce = 100;
    [SerializeField] private float rotationForce = 100;

    public void PushObject(Vector3 direction)
    {
        //Rigidbody.AddForce(transform.up * pushForce, ForceMode.Impulse);
        Rigidbody.AddTorque(direction * rotationForce, ForceMode.Force);
      
    }



    private Rigidbody rb;

    private Rigidbody Rigidbody
    {
        get 
        { 
            if(rb == null) { rb = GetComponent<Rigidbody>(); }
            return rb;
        }
    }
}
