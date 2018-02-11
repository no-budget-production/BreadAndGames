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
    public UnitTypes ThisUnityTypeFlags;

    //public UnitTypesFlags UnityTypeFlags;
    //public UnitTypes UnitType;

    //public bool HasFlag(UnitTypesFlags flags)
    //{
    //    int typeflag = 1 << (int)UnitType;
    //    return (typeflag & (int)flags) != 0;
    //}

    public float CurrentHealth;
    public float MaxHealth;
    public float Armor;
    public bool isDeadTrigger;

    public bool DestroyOnDeath;
    //public float Decay;

    public virtual void TakeDamage(float damage)
    {
        //Debug.Log("ThisUnityTypeFlags = " + (int)(ThisUnityTypeFlags));

        //Debug.Log("UnitTypes.Invurnable = " + (UnitTypes.Invurnable));
        //if (((ThisUnityTypeFlags | UnitTypes.Invurnable) != 0))
        if (ThisUnityTypeFlags > UnitTypes.Invurnable)
        {
            //Debug.Log("ThisUnityTypeFlags | UnitTypes.Invurnable = " + (int)(ThisUnityTypeFlags | UnitTypes.Invurnable));
            return;
        }

        //Debug.Log("EntityTakeDamage");

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

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
