using System.Collections.Generic;
using UnityEngine;

public class Roulette : Cheat
{
    public int KillPlayer = 0;
    public List<int> alivePlayers;
    public int randomPlayerNumber;

    public float KillDamage;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public override void Shoot()
    {
        if (KillPlayer != 0)
        {
            if (KillPlayer - 1 <= gameManager.Players.Count)
            {
                if (gameManager.Players[KillPlayer - 1].CurrentHealth > 0)
                {
                    gameManager.Players[KillPlayer - 1].TakeDamage(KillDamage, DamageType.Melee);
                }
                else
                {
                    RandomPlayerKill();
                }
            }
        }
        else
        {
            RandomPlayerKill();
        }
    }

    public void RandomPlayerKill()
    {
        for (int i = 0; i < gameManager.Players.Count; i++)
        {
            if (gameManager.Players[i].CurrentHealth > 0)
            {
                alivePlayers.Add(i);
            }
        }

        randomPlayerNumber = Random.Range(0, alivePlayers.Count);

        gameManager.Players[alivePlayers[randomPlayerNumber]].TakeDamage(KillDamage, DamageType.Melee);
    }
}
