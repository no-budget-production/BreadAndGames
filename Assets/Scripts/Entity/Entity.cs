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
    public float Armor;
    public bool isDeadTrigger;

    public bool DestroyOnDeath;
    //public float Decay;

    public virtual void TakeDamage(float damage)
    {
        //Debug.Log("EntityDamage");
        if (FlagsHelper.HasUnitTypes(ThisUnityTypeFlags, UnitTypes.Invurnable))
        {
            //Debug.Log("Invurnable");
            return;
        }

        //Debug.Log("EntityTakeDamage");

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (CurrentHealth <= 0)
        {
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
