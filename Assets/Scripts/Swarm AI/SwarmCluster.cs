using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmCluster : MonoBehaviour
{

    public float SphereRadius;
    public bool DrawCheckSphere;


    // Draw a gizmo sphere for visibilty of the checkSphere and debugging
    void OnDrawGizmosSelected()
    {
        if (DrawCheckSphere)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, SphereRadius);
        }

    }

}
