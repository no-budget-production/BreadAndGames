using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_ShellEject : MonoBehaviour
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

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifetime);

        float fadePercent = 0;
        float fadeSpeed = 1 / fadetime;

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
