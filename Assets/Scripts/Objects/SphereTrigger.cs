using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTrigger : MonoBehaviour
{
    public string[] Tags;
    public SwarmSpawn[] SwarmSpawn;
    public int playerCount;

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < Tags.Length; i++)
        {
            if (other.CompareTag(Tags[i]))
            {
                playerCount++;
            }
        }
        //Debug.Log("WRF");
        CheckPlayerCount();
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < Tags.Length; i++)
        {
            if (other.CompareTag(Tags[i]))
            {
                playerCount--;
            }
        }
        CheckPlayerCount();
    }

    void CheckPlayerCount()
    {
        if (playerCount == 0)
        {
            for (int i = 0; i < SwarmSpawn.Length; i++)
            {
                SwarmSpawn[i].enabled = false;
            }

        }
        else if (playerCount == 1)
        {
            for (int i = 0; i < SwarmSpawn.Length; i++)
            {
                SwarmSpawn[i].enabled = true;
            }

        }

    }
}
