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