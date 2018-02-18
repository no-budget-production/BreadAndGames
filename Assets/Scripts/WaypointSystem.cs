using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointSystem : MonoBehaviour
{
    public bool autoRenameWaypoints;
    public List<Transform> waypoints = new List<Transform>();

    private int Index;

	void Update ()
    {
        
        if (!Application.isPlaying)
        {
            Transform[] temp = GetComponentsInChildren<Transform>();

            if (temp.Length > 0)
            {
                waypoints.Clear();
                Index = 0;


                foreach (Transform t in temp)
                {
                    if (t != transform)
                    {
                        if (autoRenameWaypoints)
                        {
                            t.name = "Waypoint " + Index.ToString();
                        }

                        waypoints.Add(t);
                        Index++;
                    }
                }
                

            }
        }
	}



    void OnDrawGizmos()
    {
        if (waypoints.Count > 0)
        {
            Gizmos.color = Color.green;
            foreach (Transform t in waypoints)
            {
                Gizmos.DrawSphere(t.position, 1f);
            }

            Gizmos.color = Color.red;
            for (int i = 0; i < waypoints.Count-1; i++)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}
