using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotate : MonoBehaviour
{

    public Vector3 rotationSpeed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(transform.rotation * rotationSpeed);

    }
}
