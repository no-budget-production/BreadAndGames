using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvurnable : Cheat
{
    public bool areInvurnable;

    public override void Shoot()
    {
        if (!areInvurnable)
        {
            for (int i = 0; i < GameManager.Instance.Players.Count; i++)
            {
                GameManager.Instance.Players[i].ThisUnityTypeFlags = UnitTypesFlags.Invurnable;
            }
            areInvurnable = true;
        }
        else
        {
            for (int i = 0; i < GameManager.Instance.Players.Count; i++)
            {
                GameManager.Instance.Players[i].ThisUnityTypeFlags = UnitTypesFlags.Player;
            }
            areInvurnable = false;
        }

    }
}