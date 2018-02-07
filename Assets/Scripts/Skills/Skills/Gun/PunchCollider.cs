using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCollider : MonoBehaviour
{
    public List<SwarmController> enemies;

    void OnTriggerEnter(Collider other)
    {
        var temp = GetComponent<SwarmController>();
        if (temp == null)
            return;
        enemies.Add(temp);
    }

    void OnTriggerExit(Collider other)
    {
        var temp = GetComponent<SwarmController>();
        if (temp == null)
            return;
        enemies.Remove(temp);
    }
}
