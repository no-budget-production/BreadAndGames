using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform currentTarget;
    public Transform beamOrigin;
    public string axisName;

    public float healAmount = 2.0f;

    Vector3 direction;
    bool isVisible = false;



    void Start ()
    {
        lineRenderer = GetComponent<LineRenderer>();
        beamOrigin = gameObject.transform;
	}

	void FixedUpdate ()
    {
        RaycastHit hit;

        if (Physics.Raycast(beamOrigin.position, direction, out hit))
        {
            if (hit.collider.tag == "Player")
            {
                isVisible = true;

                lineRenderer.SetPosition(0, beamOrigin.position);
                lineRenderer.SetPosition(1, currentTarget.position);

                OnHealObject(hit);
            }

            else if (hit.collider.tag != "Player")
            {
                isVisible = false;

                lineRenderer.SetPosition(0, beamOrigin.position);
                lineRenderer.SetPosition(1, transform.position);
            }
        }

        if (Input.GetAxis(axisName) > 0.25f)
            {

                lineRenderer.enabled = true;
                
                LockOn();
                RaycastCheck();


            }
        else if(!isVisible)
            {
                lineRenderer.enabled = false;
            }
    }

    void LockOn()
    {
        direction = currentTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(beamOrigin.rotation, lookRotation, Time.deltaTime * 100000000).eulerAngles;
        beamOrigin.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }



    void RaycastCheck()
    {


    }

    void OnHealObject(RaycastHit hit)
    {
        Debug.Log(hit.collider.gameObject.name);
        IHealable healableObject = hit.collider.GetComponent<IHealable>();
        if (healableObject != null)
        {
            healableObject.TakeHeal(healAmount * 0.5f, hit);
        }
        else
        {
            healableObject.TakeHeal(healAmount, hit);
        }
    }
}


