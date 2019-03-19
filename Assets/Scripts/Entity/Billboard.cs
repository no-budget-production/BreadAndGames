using UnityEngine;

public class Billboard : MonoBehaviour
{
    [HideInInspector]
    public static Vector3 SouthVector;
    private Transform thisTransform;

    private void Awake()
    {
        SouthVector = GameManager._SouthVector;
        thisTransform = GetComponent<Transform>();
    }

    void Update()
    {
        Vector3 relativePos = SouthVector - thisTransform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        thisTransform.rotation = rotation;
    }
}
