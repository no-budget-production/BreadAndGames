using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueMiddlePointOfThePlayers : MonoBehaviour
{
    void Update()
    {
        transform.position = GameManager.Instance.ActiveCamera.desiredPosition;
    }
}
