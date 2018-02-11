using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity : MonoBehaviour
{
    public enum UnitTypes
    {
        Player,
        Enemy,
        Neutral,
        Invurnable
    }

    [System.Flags]
    public enum UnitTypesFlags
    {
        Player = 1 << UnitTypes.Player,
        Enemy = 1 << UnitTypes.Enemy,
        Neutral = 1 << UnitTypes.Neutral,
        Invurnable = 1 << UnitTypes.Invurnable,
    }

    public UnitTypes UnitType;

    public bool HasFlag(UnitTypesFlags flags)
    {
        int typeflag = 1 << (int)UnitType;
        return (typeflag & (int)flags) != 0;
    }

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
            //Debug.Log(gameObject + " is dead");
            isDeadTrigger = true;
        }
    }

    public virtual void GetHealth(float damage)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + damage, MaxHealth);
    }

    public virtual void ArmorBuff(float armorArg)
    {
        Armor += Mathf.Clamp(armorArg, 0, armorArg);
    }
}
