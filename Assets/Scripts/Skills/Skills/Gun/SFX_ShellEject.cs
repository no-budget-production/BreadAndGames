using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_ShellEject : MonoBehaviour
{
    public Rigidbody ShellRigidbody;
    public float ForceMin;
    public float ForceMax;

    public float Lifetime = 4;
    public float Fadetime = 0.5f;

    void Start()
    {
        float force = Random.Range(ForceMin, ForceMax);
        ShellRigidbody.AddForce(transform.right * force);
        ShellRigidbody.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(Lifetime);

        float fadePercent = 0;
        float fadeSpeed = Fadetime;

        Material mat = GetComponent<Renderer>().material;
        Color initialColor = mat.color;

        while (fadePercent < 1)
        {
            fadePercent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initialColor, Color.clear, fadePercent);
            yield return null;
        }
        Destroy(gameObject);
    }

}
