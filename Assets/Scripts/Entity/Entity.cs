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

    [EnumFlagsAttribute]
    public UnitTypesFlags ThisUnityTypeFlags;

    public float CurrentHealth;
    public float MaxHealth;

    public float HealthRegeneration;

    public float MeleeArmor;
    public float RangedArmor;

    public float MeleeArmorMultiplicator;
    public float RangeArmorMultiplicator;
    public float HealthRegenerationMultiplicator;

    public bool isDeadTrigger;

    public bool DestroyOnDeath;
    //public float Decay;

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
                Destroy(this.gameObject);
            }
        }
    }

    public virtual void OnDestroy()
    {

    }

    public virtual void GetHealth(float healing)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + healing, MaxHealth);
    }

    public virtual bool RestoreHealth(float healing)
    {
        if (CurrentHealth == MaxHealth)
            return false;
        CurrentHealth = Mathf.Min(CurrentHealth + healing, MaxHealth);
        return true;
    }
}
