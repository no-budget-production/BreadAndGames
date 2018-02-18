using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddlePointOfThePlayers : Cheat
{
    public Transform MiddlePoint;
    public Transform curMiddlePoint;

    bool isSpawned;

    public override void Shoot()
    {
        if (!isSpawned)
        {
            curMiddlePoint = Instantiate(MiddlePoint, Vector3.zero, Quaternion.identity);
            isSpawned = true;
        }
        else
        {
            Destroy(curMiddlePoint.gameObject);
            curMiddlePoint = null;
            isSpawned = false;
        }
    }
}
