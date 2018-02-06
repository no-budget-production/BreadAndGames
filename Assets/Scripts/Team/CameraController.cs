using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float DampTime = 0.2f;
    public float ScreenEdgeBuffer = 4f;
    public float MinSize = 6.5f;

    [Header("UnknownValues:")]
    public float CameraDistanceValue0;
    public float CameraDistanceValue1;

    [Header("Privates:")]
    public Transform[] targetPlayer;
    public Camera cameraGamerObject;

    private float zoomSpeed;
    private Vector3 moveVelocity;
    private Vector3 desiredPosition;

    //private void Start()
    //{
    //    SetStartPositionAndSize();
    //}

    public void Setup(Transform[] player)
    {
        targetPlayer = player;
    }

    private void FixedUpdate()
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref moveVelocity, DampTime);
    }

    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();

        int numTargets = 0;

        for (int i = 0; i < targetPlayer.Length; i++)
        {

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
<<<<<<< HEAD:Assets/Scripts/TeamScript.cs
        cameraGamerObject.fieldOfView = Mathf.SmoothDamp(cameraGamerObject.fieldOfView, requiredSize, ref zoomSpeed, dampTime);
=======

        cameraGamerObject.transform.localPosition = new Vector3(0, 0, Mathf.SmoothDamp(cameraGamerObject.transform.localPosition.z, -(requiredSize), ref zoomSpeed, DampTime));
>>>>>>> master:Assets/Scripts/Team/CameraController.cs
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

        size += ScreenEdgeBuffer;

        size = Mathf.Max(size, MinSize);

        var distance = size * CameraDistanceValue0 / Mathf.Tan(cameraGamerObject.fieldOfView * CameraDistanceValue1 * Mathf.Deg2Rad);

        return distance;
    }

    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = desiredPosition;

        cameraGamerObject.orthographicSize = FindRequiredSize();
    }
}
