﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffTypes
{
    LeftPunch,
    RightPunch,
    EnergyBarrier,
    Hook,
    Charging,
    ComboL1,
    ComboR1,
    ComboL2,

    PrimaryWeapon,
    SecondaryWeapon,
    TertiaryWeapon,
    Reload,
    HipFireMode,
    AimMode,
    PaimMode,
    PhaseRush,

    HealingMelee,
    HealingShooter,
    IsHealingMelee,
    IsHealingShooter,

    EnemyMeleeWeapon,
    EnemyRangedWeapon,
    EnemyRangedWeaponR,

    Revive,
    ReviveCooldown,

    _Length,
    _Invalid = -1

}

[CreateAssetMenu(fileName = "New Buff", menuName = "BuffObject", order = 1)]
public class BuffObject : ScriptableObject
{
    public new string name;
    public BuffTypes BuffType;
    public int ID;

    public bool isPermanent;
    public float maxTime;
    public float curTime;

    public bool isStackable;

    public BuffTypes[] cantTriggerWith;
    public BuffTypes[] effectWith;

    public bool disableWalking;
    public bool disableRightStick;
    public bool disableSkills;

    public float MeleeDamageMultiplicator;
    public float RangeDamageMultiplicator;
    public float AccuracyMultiplicator;
    public float MoveSpeedMultiplicator;

    public float MeleeArmorMultiplicator;
    public float RangeArmorMultiplicator;

    public float HealthRegenerationMultiplicator;
    public float ActionPointRegeneration;

    public float GainHealth;
    public float LoseHealth;

    public DamageType DamageType;

    public float GainActionPoints;
    public float LoseActionPoints;

    public BuffEndScript BuffEndScript;

    public bool HasEffectWith(List<ActiveBuffObject> BuffList)
    {
        for (int i = 0; i < BuffList.Count; i++)
        {
            for (int j = 0; j < effectWith.Length; j++)
            {
                if (BuffList[i].BuffObject.BuffType == effectWith[j])
                {
                    //Debug.Log("effectWithWith");
                    return true;
                }
            }
        }
        //Debug.Log("effectWith NotFound");
        return false;
    }

    public bool HasCanTriggerWith(List<ActiveBuffObject> BuffList)
    {
        for (int i = 0; i < BuffList.Count; i++)
        {
            for (int j = 0; j < cantTriggerWith.Length; j++)
            {
                if (BuffList[i].BuffObject.BuffType == cantTriggerWith[j])
                {
                    //Debug.Log("cantTriggerWith");
                    return true;
                }
            }
        }
        //Debug.Log("cantTriggerWith NotFound");
        return false;
    }

    public bool HasBuff(List<ActiveBuffObject> BuffList)
    {
        for (int i = 0; i < BuffList.Count; i++)
        {
            if (BuffList[i].BuffObject.BuffType == BuffType)
            {
                //Debug.Log("HasBuff");
                return true;

            }
        }
        //Debug.Log("HasBuff NotFound");
        return false;
    }
}
