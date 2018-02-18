using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trigger : MonoBehaviour
{
    public bool setBool;
    public Behaviour[] EnabledWithPlayer;
    public int playerCount;
    public int requiredPlayerCount;
    //public PlayerController.PlayerTypeFlags Types;

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
        CheckPlayerCount();

    }

    void CheckPlayerCount()
    {
        if (playerCount >= requiredPlayerCount)
        {
            if (setBool)
            {
                for (int i = 0; i < EnabledWithPlayer.Length; i++)
                {
                    var temp = EnabledWithPlayer[i].GetComponent<BehaviourWithBool>();
                    if (temp == null)
                        return;
                    temp.setBool = true;
                }
            }
            else
            {
                for (int i = 0; i < EnabledWithPlayer.Length; i++)
                {
                    EnabledWithPlayer[i].enabled = true;
                }
            }


        }
        else
        {
            if (setBool)
            {
                for (int i = 0; i < EnabledWithPlayer.Length; i++)
                {
                    var temp = EnabledWithPlayer[i].GetComponent<BehaviourWithBool>();
                    if (temp == null)
                        return;
                    temp.setBool = false;
                }
            }
            else
            {
                for (int i = 0; i < EnabledWithPlayer.Length; i++)
                {
                    if (setBool)
                    {
                        EnabledWithPlayer[i].enabled = false;
                    }
                }
            }
        }
    }
}
