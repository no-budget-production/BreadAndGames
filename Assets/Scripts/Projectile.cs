using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    float speed = 10;
    float lifetime = 2;
    float fadetime = 2;

    public void SetSpeed (float newSpeed)
    {
        speed = newSpeed;

        StartCoroutine(Fade());
    }

	void Update ()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
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
