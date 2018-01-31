using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableSet : MonoBehaviour
{
    public float xScale = 10;
    public float zScale = 10;
    public int intensity = 25;

    public ParticleSystem[] ParticleComponents;

    // Use this for initialization
    void Start () {
        foreach (var ParticleSystemRef in ParticleComponents)
        {
            ParticleSystemRef.shape.scale.Set(xScale, 1, zScale);
            var emission = ParticleSystemRef.emission;
            emission.rateOverTime = intensity;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
