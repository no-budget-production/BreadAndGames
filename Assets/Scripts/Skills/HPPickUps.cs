using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPPickUps : MonoBehaviour
{
    public float healthAmountInPercent;

    void OnTriggerEnter(Collider other)
    {
        var temp = other.GetComponent<PlayerController>();
        if (temp == null)
        {
            return;
        }
        var used = temp.RestoreHealth(temp.MaxHealth/100 * healthAmountInPercent);
        if (used)
        {
            Destroy(this.gameObject);
        }
    }
}
