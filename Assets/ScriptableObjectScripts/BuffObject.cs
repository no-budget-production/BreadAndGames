using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffTypes
{
    LeftPunch,
    RightPunch,
    EnergyBarrier,
    Hook,
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
    HealingDrone,
    HealingDroneReturn,

    _Length,
    _Invalid = -1

}

[CreateAssetMenu(fileName = "New Buff", menuName = "BuffObject", order = 1)]
public class BuffObject : ScriptableObject
{
    public new string name;
    public BuffTypes BuffTypes;
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

    public float MeleeDamageMultiplicator = 1;
    public float RangeDamageMultiplicator = 1;
    public float AccuracyMultiplicator = 1;
    public float MoveSpeedMultiplicator = 1;

    public float MeleeArmorMultiplicator = 1;
    public float RangedArmorMultiplicator = 1;

    public float GainHealth;
    public float LoseHealth;

    public float GainActionPoints;
    public float LoseGainActionPoints;

}

