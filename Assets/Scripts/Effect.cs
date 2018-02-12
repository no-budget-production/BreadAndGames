using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public float Lifetime = 4f;
    public float Fadetime = 0.25f;

    protected virtual IEnumerator Fade()
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


