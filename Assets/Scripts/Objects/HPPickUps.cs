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

        float playerControllerInitialHealth = PlayerController.CurrentHealth;
        var used = PlayerController.RestoreHealth(PlayerController.MaxHealth / 100 * healthAmountInPercent);
        if (used)
        {
            GameManager.Instance.HealthPickUps.Remove(this);
            Destroy(this.gameObject);
            StatsTracker.Instance.HealthPacks[PlayerController.InternalPlayerNumber]++;
            StatsTracker.Instance.Healed[PlayerController.InternalPlayerNumber] += PlayerController.CurrentHealth - playerControllerInitialHealth;
        }
    }
}
