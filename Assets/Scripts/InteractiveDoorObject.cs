using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveDoorObject : MonoBehaviour
{
    [SerializeField] private GameObject hingeObject;
    [SerializeField] private InteractiveDoorObject otherDoorKnobObject;
    [SerializeField] private float rotationMax = 90.0f;
    [SerializeField] private float openSpeed = 20.0f;
    [SerializeField] private bool isOpen = false;
    void Update()
    {
        Open();
    }
    private void Open()
    {
        if (!activated) return;

        Quaternion endRotation = isOpen ? Quaternion.identity : Quaternion.Euler(0, rotationMax, 0);

        hingeObject.transform.rotation = Quaternion.RotateTowards(hingeObject.transform.rotation, endRotation, Time.deltaTime * openSpeed);
        if (hingeObject.transform.rotation == endRotation)
        {
            activated = false;
            isOpen = !isOpen;
            if(otherDoorKnobObject)
                otherDoorKnobObject.isOpen = isOpen;
        }

    }
    public void OpenObject()
    {
        activated = true;
    }
    private bool activated = false;

    public bool IsOpen
    {
        get { return isOpen; }
        set { isOpen = value; }
    }
}
