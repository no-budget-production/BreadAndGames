using UnityEngine;

public class SFX_ShellEject : Effect
{
    public Rigidbody ShellRigidbody;
    public float ForceMin;
    public float ForceMax;

    void Start()
    {
        float force = Random.Range(ForceMin, ForceMax);
        ShellRigidbody.AddForce(transform.right * force);
        ShellRigidbody.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
    }
}
