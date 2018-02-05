using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_ShellEject : Effect
{
    public Rigidbody shellRigidbody;
    public float forceMin;
    public float forceMax;

    float lifetime = 4;
    float fadetime = 2;

    void Start()
    {
        float force = Random.Range(forceMin, forceMax);
        shellRigidbody.AddForce(transform.right * force);
        shellRigidbody.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine (Fade());
    }

}
