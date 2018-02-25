using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IFade : MonoBehaviour
{
    public float Lifetime = 4f;
    public float Fadetime = 0.01f;
    public Renderer Renderer;
    public Material TargetMaterial;

    public virtual IEnumerator Fade()
    {
        Debug.Log("StartKilling!");

        yield return new WaitForSeconds(Lifetime);

        Debug.Log("KillIt!");

        if (Renderer != null)
        {
            if (TargetMaterial != null)
            {

            }

            var mat = Renderer.material;

            float fadePercent = 0.0f;
            float fadeSpeed = Fadetime;
            while (fadePercent < fadeSpeed)
            {
                fadePercent += Time.deltaTime;
                Renderer.material.Lerp(mat, TargetMaterial, fadePercent / Fadetime);
                yield return null;
            }
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
}


