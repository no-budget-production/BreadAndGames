using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    private int playerCount;
    public int requiredPlayerCount;

    private GameManager gameManager;
    private StatsTracker statsTracker;

    private void Start()
    {
        gameManager = GameManager.Instance;
        statsTracker = StatsTracker.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        var temp = other.GetComponent<PlayerController>();
        if (temp == null)
            return;

        playerCount++;
        CheckPlayerCount();
    }

    private void OnTriggerExit(Collider other)
    {
        var temp = other.GetComponent<PlayerController>();
        if (temp == null)
            return;

        playerCount--;
    }

    void CheckPlayerCount()
    {
        if (playerCount >= requiredPlayerCount)
        {
            AddStats();

            if (gameManager != null)
            {
                if (gameManager.InstanceRef != null)
                {
                    gameManager.transform.parent = gameManager.InstanceRef.transform;

                    Destroy(gameManager.InstanceRef.gameObject);
                }
            }

            SceneManager.LoadScene("Game Over", LoadSceneMode.Single);
        }
    }

    void AddStats()
    {
        statsTracker.Wins++;

        statsTracker.Time = Mathf.Round(Time.timeSinceLevelLoad * 10.0f) / 10.0f;

        statsTracker.CalculateBestTimes();
    }
}
