using UnityEngine;

public class HPPickUps : MonoBehaviour
{
    public float healthAmountInPercent;

    private Transform thisTransform;

    private GameManager gameManager;
    private StatsTracker statsTracker;

    private void Awake()
    {
        thisTransform = GetComponent<Transform>();
        gameManager = GameManager.Instance;
        statsTracker = StatsTracker.Instance;
    }

    public Transform GetTransform()
    {
        return thisTransform;
    }

    void OnTriggerEnter(Collider other)
    {
        var PlayerController = other.GetComponent<PlayerController>();
        if (PlayerController == null)
        {
            return;
        }

        float playerControllerInitialHealth = PlayerController.CurrentHealth;

        if (playerControllerInitialHealth <= 0)
        {
            return;
        }

        var used = PlayerController.RestoreHealth(PlayerController.MaxHealth / 100 * healthAmountInPercent);
        if (used)
        {
            gameManager.HealthPickUps.Remove(this);
            Destroy(this.gameObject);
            statsTracker.HealthPacks[PlayerController.InternalPlayerNumber]++;
            statsTracker.Healed[PlayerController.InternalPlayerNumber] += PlayerController.CurrentHealth - playerControllerInitialHealth;
        }
    }
}
