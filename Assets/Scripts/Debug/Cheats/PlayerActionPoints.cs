using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionPoints : Cheat
{
    public float ActionPointsAmount;

    public override void Shoot()
    {
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            PlayerController curPlayer = GameManager.Instance.Players[i].GetComponent<PlayerController>();

            curPlayer.RestoreActionPoints(ActionPointsAmount);
        }

        //Debug.Log("PlayersHealed: " + ActionPointsAmount);
    }
}
