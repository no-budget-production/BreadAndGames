using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPPickUps : MonoBehaviour
{
    public float healthAmountInPercent;

    void OnTriggerEnter(Collider other)
    {
        var PlayerController = other.GetComponent<PlayerController>();
        if (PlayerController == null)
        {
            return;
        }
        GameManager.Instance.HealthPickUps.Remove(this);
        var used = PlayerController.RestoreHealth(PlayerController.MaxHealth / 100 * healthAmountInPercent);
        if (used)
        {
            Destroy(this.gameObject);
            StatsTracker.Instance.HealthPacks[PlayerController.InternalPlayerNumber]++;
        }
    }
}
