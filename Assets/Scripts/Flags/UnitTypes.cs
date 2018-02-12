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

//[System.Flags]
//public enum UnitTypes
//{
//    Player,
//    Enemy,
//    Neutral,
//    Invurnable
//}