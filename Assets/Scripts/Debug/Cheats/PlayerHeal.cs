using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeal : Cheat
{
    public float HealAmount;

    public override void Shoot()
    {
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            PlayerController curPlayer = GameManager.Instance.Players[i].GetComponent<PlayerController>();

            curPlayer.GetHealth(HealAmount);
        }

        //Debug.Log("PlayersHealed: " + HealAmount);
    }
}
