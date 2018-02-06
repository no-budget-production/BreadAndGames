using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    private LineRenderer lineRenderer;

	void Start ()
    {
        lineRenderer = GetComponent<LineRenderer>();	
	}
	
	void Update ()
    {
        RaycastHit Hit;

        if (Physics.Raycast(transform.position, transform.forward, out Hit))
        {
            if(Hit.collider)
            {
                lineRenderer.SetPosition(1, new Vector3(0, 0, Hit.distance));
            }
        }
        else
        {
            lineRenderer.SetPosition(1, new Vector3(0, 0, 20));
        }
	}
}
