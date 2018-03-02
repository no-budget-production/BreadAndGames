using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    private int playerCount;
    public int requiredPlayerCount;

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

            SceneManager.LoadScene("Game Over", LoadSceneMode.Single);
        }
    }

    void AddStats()
    {
        StatsTracker.Instance.Wins++;

        StatsTracker.Instance.Time = Mathf.Round(Time.timeSinceLevelLoad * 10.0f) / 10.0f;

        StatsTracker.Instance.CalculateBestTimes();
    }
}
