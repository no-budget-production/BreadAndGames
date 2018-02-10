using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : Cheat
{
    public PlayerController NewPlayer1;
    public PlayerController.PlayerType TargetType;

    public override void Shoot()
    {
        if (NewPlayer1 == null)
        {
            NewPlayer1 = GameManager.Instance.GetPlayerByType(TargetType);
        }

        int j = 2;

        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            PlayerController curPlayer = GameManager.Instance.Players[i].GetComponent<PlayerController>();

            if (curPlayer != GameManager.Instance.GetPlayerByType(TargetType))
            {
                curPlayer.PlayerNumber = j.ToString();
                j++;
            }
            else
            {
                curPlayer.PlayerNumber = "1";
            }

            curPlayer.Setup(GameManager.Instance.InputRotation, GameManager.Instance.prefabLoader.ButtonStrings);
        }

        Debug.Log(NewPlayer1.name + "new Player 1");
    }
}
