using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamCamera : MonoBehaviour
{

    public Transform[] cameraTargets;

    float xMax;
    float zMax;

    float xMin;
    float zMin;

    float xCenter;
    float zCenter;

    public float cameraHeight;

    Vector3 centerPoint;
    

	// Use this for initialization
	void Start ()
    {
		
	}
	
	void Update ()
    {


	}

    void FindCenter()
    {
        xCenter = xMin + (Mathf.Abs(xMax - xMin) / 2.0f);
        zCenter = zMin + (Mathf.Abs(zMax - zMin) / 2.0f);

        //cameraHeight = 90.0f / Camera.fieldOf

        centerPoint = new Vector3(xCenter, cameraHeight , zCenter);


    }
}
