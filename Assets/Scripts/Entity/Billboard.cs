using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [HideInInspector]
    public static Vector3 SouthVector;

    private void Awake()
    {
        SouthVector = GameManager._SouthVector;
    }

    void Update()
    {
        Vector3 relativePos = SouthVector - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = rotation;
    }
}
