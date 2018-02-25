using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public List<int> ReturnSelectedElements()
    {
        List<int> selectedElements = new List<int>();
        for (int i = 0; i < System.Enum.GetValues(typeof(UnitTypes)).Length; i++)
        {
            int layer = 1 << i;
            if (((int)ThisUnityTypeFlags & layer) != 0)
            {
                selectedElements.Add(i);
            }
        }
        return selectedElements;
    }

    [Header(">>>>>>>>>> ENTITY:")]

    [EnumFlagsAttribute]
    public UnitTypesFlags ThisUnityTypeFlags;

    public float CurrentHealth;
    public float MaxHealth;

    public float HealthRegeneration;

    public float MeleeArmor;
    public float RangedArmor;

    [HideInInspector]
    public float MeleeArmorMultiplicator;
    [HideInInspector]
    public float RangeArmorMultiplicator;
    //[HideInInspector]
    public float HealthRegenerationMultiplicator;

    public bool isDeadTrigger;
    public bool hasDied;
    public bool DestroyOnDeath;


    public virtual void TakeDamage(float damage, DamageType damageType)
    {
        if (FlagsHelper.HasUnitTypes(ThisUnityTypeFlags, UnitTypes.Invurnable))
        {
            return;
        }

        float editedDamage = damage;

        if (damageType == DamageType.Melee)
        {
            editedDamage = Mathf.Max(editedDamage - MeleeArmor - (MeleeArmor * MeleeArmorMultiplicator), 0);
        }
        else if (damageType == DamageType.Ranged)
        {
            editedDamage = Mathf.Max(editedDamage - RangedArmor - (RangedArmor * RangeArmorMultiplicator), 0);
        }

        CurrentHealth = Mathf.Max(CurrentHealth - editedDamage, 0);

        if (CurrentHealth <= 0)
        {
            isDeadTrigger = true;

            if (DestroyOnDeath)
            {
                if (!hasDied)
                {
                    OnCustomDestroy();
                }
            }
        }
    }

    //int DiedAmount;

    public virtual void OnCustomDestroy()
    {
        //DiedAmount++;
        hasDied = true;
        //Debug.Log(DiedAmount);
        Destroy(gameObject);
    }

    public virtual void GetHealth(float healing)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + healing, MaxHealth);

        if (CurrentHealth > 0)
        {
            isDeadTrigger = false;
        }
    }

    public virtual bool RestoreHealth(float healing)
    {
        if (CurrentHealth == MaxHealth)
        {
            return false;
        }
        GetHealth(healing);

        return true;
    }
}
