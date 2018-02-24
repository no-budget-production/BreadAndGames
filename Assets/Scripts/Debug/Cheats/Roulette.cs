using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roulette : Cheat
{
    public int KillPlayer = 0;
    public List<int> alivePlayers;
    public int randomPlayerNumber;

    public float KillDamage;


    public override void Shoot()
    {
        if (KillPlayer != 0)
        {
            if (KillPlayer - 1 <= GameManager.Instance.Players.Count)
            {
                GameManager.Instance.Players[KillPlayer - 1].TakeDamage(KillDamage, DamageType.Melee);
            }
        }
        else
        {
            for (int i = 0; i < GameManager.Instance.Players.Count; i++)
            {
                if (GameManager.Instance.Players[i].CurrentHealth >= 0)
                {
                    alivePlayers.Add(i);
                }
            }

            randomPlayerNumber = Random.Range(0, alivePlayers.Count);

            GameManager.Instance.Players[alivePlayers[randomPlayerNumber]].TakeDamage(KillDamage, DamageType.Melee);
        }
    }
}
