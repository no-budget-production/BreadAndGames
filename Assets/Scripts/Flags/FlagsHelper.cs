public enum UnitTypes
{
    Player,
    Enemy,
    Neutral,
    Invurnable,
    _Length,
    _Invalid = -1
}

public enum AnimTypes
{
    Trigger,
    Bool,
    Float,
    _Length,
    _Invalid = -1
}

public enum Anims
{
    MovX,
    MovY,
    Aim_Amount,
    isDead,
    GetUp,
    isRunning,
    isAiming,
    Skill_0,
    Skill_1,
    Skill_2,
    Skill_Bool_0,
    _Length,
    _Invalid = -1
}

[System.Flags]
public enum UnitTypesFlags
{
    Player = 1 << UnitTypes.Player,
    Enemy = 1 << UnitTypes.Enemy,
    Neutral = 1 << UnitTypes.Neutral,
    Invurnable = 1 << UnitTypes.Invurnable,
}

public enum DamageType
{
    Melee,
    Ranged,
    Skill,
    Trigger,
    _Length,
    _Invalid = -1
}

[System.Flags]
public enum DamageTypeFlags
{
    Melee = 1 << DamageType.Melee,
    Ranged = 1 << DamageType.Ranged,
    Skill = 1 << DamageType.Skill,
    Trigger = 1 << DamageType.Trigger
}


public enum PlayerType
{
    Melee,
    Shooter,
    Support,
    _Length
}

[System.Flags]
public enum PlayerTypeFlags
{
    Melee = 1 << PlayerType.Melee,
    Shooter = 1 << PlayerType.Shooter,
    Support = 1 << PlayerType.Support,
}


public enum SkillType
{
    LeftPunch,
    RightPunch,
    EnergyBarrier,
    Hook,
    Charging,
    ComboL1,
    ComboR1,
    ComboL2,
    Gun,
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
    EnemyMeleeWeapon,
    EnemyRangedWeapon,
    Revive,
    ReviveCooldown,
    _Length,
    _Invalid = -1
}

[System.Flags]
public enum SkillTypeFlags
{
    LeftPunch = 1 << SkillType.LeftPunch,
    RightPunch = 1 << SkillType.RightPunch,
    EnergyBarrier = 1 << SkillType.EnergyBarrier,
    Hook = 1 << SkillType.Hook,
    Charging = 1 << SkillType.Charging,
    ComboL1 = 1 << SkillType.ComboL1,
    ComboR1 = 1 << SkillType.ComboR1,
    ComboL2 = 1 << SkillType.ComboL2,
    Gun = 1 << SkillType.Gun,
    PrimaryWeapon = 1 << SkillType.PrimaryWeapon,
    SecondaryWeapon = 1 << SkillType.SecondaryWeapon,
    TertiaryWeapon = 1 << SkillType.TertiaryWeapon,
    Reload = 1 << SkillType.Reload,
    HipFireMode = 1 << SkillType.HipFireMode,
    AimMode = 1 << SkillType.AimMode,
    PaimMode = 1 << SkillType.PaimMode,
    PhaseRush = 1 << SkillType.PhaseRush,
    HealingMelee = 1 << SkillType.HealingMelee,
    HealingShooter = 1 << SkillType.HealingShooter,
    HealingDrone = 1 << SkillType.HealingDrone,
    HealingDroneReturn = 1 << SkillType.HealingDroneReturn,
    EnemyMeleeWeapon = 1 << SkillType.HealingDrone,
    EnemyRangedWeapon = 1 << SkillType.EnemyRangedWeapon,
    Revive = 1 << SkillType.Revive,
    ReviveCooldown = 1 << SkillType.ReviveCooldown
}

public enum BuffType
{
    CanWalk,
    CanUseRightStick,
    CanUseSkills,
    _Length,
    _Invalid = -1
}

[System.Flags]
public enum BuffTypeFlags
{
    canWalk = 1 << BuffType.CanWalk,
    canUseRightStick = 1 << BuffType.CanUseRightStick,
    canUseSkills = 1 << BuffType.CanUseSkills
}


class FlagsHelper
{
    public static bool HasUnitTypes(UnitTypesFlags flags, UnitTypes enu)
    {
        int typeflag = 1 << (int)enu;
        return (typeflag & (int)flags) != 0;
    }


    public static bool HasUnitTypes(UnitTypesFlags flags, UnitTypesFlags flags2)
    {
        return ((int)flags2 & (int)flags) != 0;
    }
}