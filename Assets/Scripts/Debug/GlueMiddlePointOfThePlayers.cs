using UnityEngine;

public class GlueMiddlePointOfThePlayers : MonoBehaviour
{
    private Transform thisTransform;

    private void Awake()
    {
        thisTransform = GetComponent<Transform>();
    }

    void Update()
    {
        thisTransform.position = GameManager.Instance.ActiveCamera.desiredPosition;
    }
}
