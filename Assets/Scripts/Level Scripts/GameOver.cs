using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour {

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

        }
    }
}
