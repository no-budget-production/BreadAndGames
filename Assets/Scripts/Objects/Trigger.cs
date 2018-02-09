using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trigger : MonoBehaviour
{
    //public string[] Tags;
    public Behaviour[] EnabledWithPlayer;
    public int playerCount;
    public PlayerController.PlayerTypeFlags Types;

    private void OnTriggerEnter(Collider other)
    {
        var temp = other.GetComponent<PlayerController>();
        if (temp == null)
            return;

        if (temp.HasFlag(Types))
        {
            playerCount++;
            CheckPlayerCount();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var temp = other.GetComponent<PlayerController>();
        if (temp == null)
            return;

        if (temp.HasFlag(Types))
        {
            playerCount--;
            CheckPlayerCount();
        }
    }

    void CheckPlayerCount()
    {
        if (playerCount == 0)
        {
            for (int i = 0; i < EnabledWithPlayer.Length; i++)
            {
                EnabledWithPlayer[i].enabled = false;
            }

        }
        else if (playerCount == 1)
        {
            for (int i = 0; i < EnabledWithPlayer.Length; i++)
            {
                EnabledWithPlayer[i].enabled = true;
            }

        }

    }
}
