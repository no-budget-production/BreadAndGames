public enum UnitTypes
{
    Player,
    Enemy,
    Neutral,
    Invurnable,
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

public enum PlayerType
{
    Melee,
    Shooter,
    Support
}

[System.Flags]
public enum PlayerTypeFlags
{
    Melee = 1 << PlayerType.Melee,
    Shooter = 1 << PlayerType.Shooter,
    Support = 1 << PlayerType.Support,
}

public enum CooldownType
{
    CoolDown0, CoolDown1, CoolDown2, CoolDown3
}

[System.Flags]
public enum CooldownTypeFlags
{
    CoolDown0 = 1 << CooldownType.CoolDown0,
    CoolDown1 = 1 << CooldownType.CoolDown1,
    CoolDown2 = 1 << CooldownType.CoolDown2,
    CoolDown3 = 1 << CooldownType.CoolDown3
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