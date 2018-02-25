using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainAreaOfEffect : MonoBehaviour
{
    public float xScale = 10f;
    public float zScale = 10f;
    public ParticleSystemShapeType newShape = ParticleSystemShapeType.Box;
    public int intensity = 25;

    public ParticleSystem[] ParticleComponents; 
    // Use this for initialization
    void Start () {
        foreach (var ParticleSystemRef in ParticleComponents)
        {
            Vector3 newSize = new Vector3(xScale, 1.0f, zScale);

            var shape = ParticleSystemRef.shape;
            shape.shapeType = newShape;
            shape.scale = newSize;
            

            var emission = ParticleSystemRef.emission;
            emission.rateOverTime = intensity;

        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
