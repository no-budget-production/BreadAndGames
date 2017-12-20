using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamScript : MonoBehaviour
{
    public float juriisgay;
    public float jurisuxdix;

    public float dampTime = 0.2f;
    public float screenEdgeBuffer = 4f;
    public float minSize = 6.5f;
    public Transform[] targetPlayer;

    public Camera cameraGamerObject;
    public float zoomSpeed;
    public Vector3 moveVelocity;
    public Vector3 desiredPosition;
    private void FixedUpdate()
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref moveVelocity, dampTime);
    }
    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < targetPlayer.Length; i++)
        {
            if (!targetPlayer[i].gameObject.activeSelf)
                continue;

            averagePos += targetPlayer[i].position;
            numTargets++;
        }

        if (numTargets > 0)
        {
            averagePos /= numTargets;
        }

        averagePos.y = transform.position.y;

        desiredPosition = averagePos;
    }
    private void Zoom()
    {
        float requiredSize = FindRequiredSize();

        //Matrix4x4 m = cameraGamerObject.cameraToWorldMatrix;
        //Vector3 p = m.MultiplyPoint(new Vector3(transform.position.x, transform.position.y, transform.position.z));

        //cameraGamerObject.transform.localPosition = new Vector3(0, 0, 0);

        ////Debug.Log(requiredSize);
        ////Debug.Log(Mathf.SmoothDamp(cameraGamerObject.orthographicSize, requiredSize, ref zoomSpeed, dampTime));
        

        cameraGamerObject.transform.localPosition = new Vector3(0, 0, Mathf.SmoothDamp(cameraGamerObject.transform.localPosition.z, -(requiredSize), ref zoomSpeed, dampTime));
    }

    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(desiredPosition);

        float size = 0f;

        for (int i = 0; i < targetPlayer.Length; i++)
        {
            if (!targetPlayer[i].gameObject.activeSelf)
            {
                continue;
            }

            Vector3 targetLocalPos = transform.InverseTransformPoint(targetPlayer[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / cameraGamerObject.aspect);
        }

        size += screenEdgeBuffer;

        size = Mathf.Max(size, minSize);

        var distance = size * juriisgay / Mathf.Tan(cameraGamerObject.fieldOfView * jurisuxdix * Mathf.Deg2Rad);

        return distance;
    }

    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = desiredPosition;

        cameraGamerObject.orthographicSize = FindRequiredSize();
    }
}
