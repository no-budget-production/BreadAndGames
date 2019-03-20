using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float DampTime = 0.2f;
    //public float[] ScreenEdgeBuffers = new float[4];
    public float ScreenEdgeBuffer = 4f;
    public float MinSize = 6.5f;

    [Header("UnknownValues:")]
    public float CameraDistanceValue0;
    public float CameraDistanceValue1;

    [Header("Privates:")]
    public Transform[] TargetPlayer;
    public Camera CameraGamerObject;

    private float ZoomSpeed;

    private Vector3 moveVelocity;
    public Vector3 desiredPosition;

    public float FogStarAdd;
    public float FogEndAdd;

    public float FogStartMult;
    public float FogEndMult;

    //private void Start()
    //{
    //    SetStartPositionAndSize();
    //}

    public void Setup(Transform[] player)
    {
        TargetPlayer = player;
        SetStartPositionAndSize();
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

        for (int i = 0; i < TargetPlayer.Length; i++)
        {
            //continue;

            averagePos += TargetPlayer[i].position;
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

        CameraGamerObject.transform.localPosition = new Vector3(0, 0, Mathf.SmoothDamp(CameraGamerObject.transform.localPosition.z, -(requiredSize), ref ZoomSpeed, DampTime));
    }

    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(desiredPosition);

        float size = 0f;

        for (int i = 0; i < TargetPlayer.Length; i++)
        {
            if (!TargetPlayer[i].gameObject.activeSelf)
            {
                continue;
            }

            Vector3 targetLocalPos = transform.InverseTransformPoint(TargetPlayer[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / CameraGamerObject.aspect);
        }

        size += ScreenEdgeBuffer;

        size = Mathf.Max(size, MinSize);

        var distance = size * CameraDistanceValue0 / Mathf.Tan(CameraGamerObject.fieldOfView * CameraDistanceValue1 * Mathf.Deg2Rad);


        RenderSettings.fogStartDistance = distance * FogStartMult + FogStarAdd;
        RenderSettings.fogEndDistance = distance * FogEndMult + FogEndAdd;

        return distance;

    }

    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = desiredPosition;

        CameraGamerObject.orthographicSize = FindRequiredSize();
    }
}
