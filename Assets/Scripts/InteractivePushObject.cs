using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractivePushObject : MonoBehaviour
{
    [SerializeField] private float pushForce = 100;

    public void PushObject(Vector3 direction)
    {
        //Rigidbody.AddForce(transform.up * pushForce, ForceMode.Impulse);
        Rigidbody.AddTorque(direction * pushForce, ForceMode.Force);
      
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
