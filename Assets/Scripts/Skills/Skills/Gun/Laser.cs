using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    public LineRenderer LineRenderer;

    void Update()
    {
        RaycastHit Hit;

        if (Physics.Raycast(transform.position, transform.forward, out Hit))
        {
            if (Hit.collider)
            {
                LineRenderer.SetPosition(1, new Vector3(0, 0, Hit.distance));
            }
        }
        else
        {
            LineRenderer.SetPosition(1, new Vector3(0, 0, 20));
        }
    }
}
