public class PlayerInvurnable : Cheat
{
    public bool areInvurnable;

    public float movementSpeedBonus;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    public override void Shoot()
    {
        if (!areInvurnable)
        {
            for (int i = 0; i < gameManager.Players.Count; i++)
            {
                gameManager.Players[i].ThisUnityTypeFlags = UnitTypesFlags.Invurnable;

                gameManager.Players[i].moveSpeedMax *= movementSpeedBonus;
            }
            areInvurnable = true;
        }
        else
        {
            for (int i = 0; i < gameManager.Players.Count; i++)
            {
                gameManager.Players[i].ThisUnityTypeFlags = UnitTypesFlags.Player;

                gameManager.Players[i].moveSpeedMax /= movementSpeedBonus;
            }
            areInvurnable = false;
        }
    }
}