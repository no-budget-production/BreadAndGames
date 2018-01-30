using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [HideInInspector]
    public static Vector3 _SouthVector;

    private void Start()
    {
        _SouthVector = new Vector3(0, 0, 10000);
    }

    void Update()
    {
        Vector3 relativePos = _SouthVector - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = rotation;
    }
}
