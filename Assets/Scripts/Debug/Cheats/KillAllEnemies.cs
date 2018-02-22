using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAllEnemies : Cheat
{
    public float TakeDamage;

    public override void Shoot()
    {
        for (int i = 0; i < GameManager.Instance.Enemies.Count; i++)
        {
            GameManager.Instance.Enemies[i].TakeDamage(TakeDamage, DamageType.Melee);
        }
        GameManager.Instance.Enemies.Clear();
    }
}
