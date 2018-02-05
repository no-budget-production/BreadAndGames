using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{

    float lifetime = 2;
    float fadetime = 2;

    protected virtual IEnumerator Fade()
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


