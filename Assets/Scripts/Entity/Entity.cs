using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public float CurrentHealth;
    public float MaxHealth;
    public float Armor;
    public bool isDeadTrigger;

    public bool DestroyOnDeath;
    //public float Decay;

    public virtual void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            Debug.Log(gameObject + " is dead");
            isDeadTrigger = true;
        }
    }

    public virtual void ArmorBuff(float armorArg)
    {
        Armor += Mathf.Clamp(armorArg, 0, armorArg);
    }
}
