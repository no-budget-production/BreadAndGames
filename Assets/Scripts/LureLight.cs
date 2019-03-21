using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LureLight : MonoBehaviour {

    private Light LureLightLight;
    private IEnumerator coroutine;

    void Start ()
    {
        LureLightLight = GetComponent<Light>();
        coroutine = WaitAndPrint(.7f);
        StartCoroutine(coroutine);
    }

    private IEnumerator WaitAndPrint(float waitTime)
    {

        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            LureLightLight.enabled = !LureLightLight.enabled;
        }
    }
}
