using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private Transform thisTransform;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        thisTransform = GetComponent<Transform>();
    }

    void Update()
    {
        RaycastHit Hit;

        if (Physics.Raycast(thisTransform.position, thisTransform.forward, out Hit))
        {
            if (Hit.collider)
            {
                lineRenderer.SetPosition(1, new Vector3(0, 0, Vector3.Distance(thisTransform.position, Hit.transform.position)) * 10);
            }
        }
        else
        {
            lineRenderer.SetPosition(1, new Vector3(0, 0, 1500));
        }
    }
}
