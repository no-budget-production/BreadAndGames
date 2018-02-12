using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    public LineRenderer LineRenderer;

    public int MaxDistance;

    public int EveryXFrames;
    private int FrameCounter;

    void Update()
    {
        FrameCounter++;
        if ((FrameCounter % EveryXFrames) == 0)
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
                LineRenderer.SetPosition(1, new Vector3(0, 0, MaxDistance));
            }

            FrameCounter = 0;
        }
    }
}
