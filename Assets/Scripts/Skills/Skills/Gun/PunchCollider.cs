using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCollider : MonoBehaviour
{
    public List<Character> enemies;

    void OnTriggerEnter(Collider other)
    {
        var temp = other.GetComponent<Character>();
        if (temp == null)
            return;
        enemies.Add(temp);
    }

    void OnTriggerExit(Collider other)
    {
        var temp = other.GetComponent<Character>();
        if (temp == null)
            return;
        enemies.Remove(temp);
    }
}
