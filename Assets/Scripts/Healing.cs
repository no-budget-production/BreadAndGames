using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{

    public LineRenderer lineRenderer;
    public Transform currentTarget;
    public Transform beamOrigin;
    public string axisName;

	void Start ()
    {
        lineRenderer = GetComponent<LineRenderer>();
        beamOrigin = gameObject.transform;
	}

	void Update ()
    {
        if (Input.GetAxis(axisName) > 0.25f)
            {

                lineRenderer.enabled = true;
                
                LockOn();
                Beam();
            }
        else
            {
                lineRenderer.enabled = false;
            }
    }

    void LockOn()
    {
        Vector3 direction = currentTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(beamOrigin.rotation, lookRotation, Time.deltaTime * 100000000).eulerAngles;
        beamOrigin.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void Beam()
    {
        lineRenderer.SetPosition(0, beamOrigin.position);
        lineRenderer.SetPosition(1, currentTarget.position);
    }

    void RayCast()
    {

    }
}


