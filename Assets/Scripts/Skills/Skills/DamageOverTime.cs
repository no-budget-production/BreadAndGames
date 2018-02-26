using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    public float Damage;
    public DamageType DamageType;

    public PunchCollider HitBox;

    void Update()
    {

        for (int i = 0; i < HitBox.Enemies.Count; i++)
        {
            if (HitBox.Enemies[i] == null)
            {
                HitBox.Enemies.Remove(HitBox.Enemies[i]);
                i--;
                continue;
            }

            HitBox.Enemies[i].TakeDamage(Damage * Time.deltaTime, DamageType);
        }
    }
}
