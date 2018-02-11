using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public UnitTypesFlags UnityTypeFlags;
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
        if (UnitType == UnitTypes.Invurnable)
        {
            return;
        }

        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            //Debug.Log(gameObject + " is dead");
            isDeadTrigger = true;
        }
    }

    public virtual void GetHealth(float healing)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + healing, MaxHealth);
    }

    public virtual void ArmorBuff(float armorArg)
    {
        Armor += Mathf.Clamp(armorArg, 0, armorArg);
    }
}
