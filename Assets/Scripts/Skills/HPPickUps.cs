using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPPickUps : MonoBehaviour
{
    public float healthAmount;
    void OnTriggerEnter(Collider other)
    {
        var temp = other.GetComponent<PlayerController>();
        if (temp == null) return;
        temp.GetHealth(healthAmount);
    }
}
