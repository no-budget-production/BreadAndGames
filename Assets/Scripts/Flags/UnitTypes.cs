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