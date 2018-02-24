using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeal : Cheat
{
    public float HealAmount;

    public override void Shoot()
    {
        int areRevived = 0;
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            PlayerController curPlayer = GameManager.Instance.Players[i].GetComponent<PlayerController>();

            curPlayer.GetHealth(HealAmount);
            if (curPlayer.CurrentHealth > 0)
            {
                areRevived++;
            }
        }

        if (areRevived == GameManager.Instance.Players.Count)
        {
            for (int i = 0; i < GameManager.Instance.Enemies.Count; i++)
            {
                GameManager.Instance.Enemies[i].isGameOver = false;
            }
        }

        Debug.Log("PlayersHealed: " + HealAmount);
    }
}
