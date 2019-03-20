public class PlayerHeal : Cheat
{
    public float HealAmount;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    public override void Shoot()
    {
        int areRevived = 0;
        for (int i = 0; i < gameManager.Players.Count; i++)
        {
            PlayerController curPlayer = gameManager.Players[i].GetComponent<PlayerController>();

            curPlayer.GetHealth(HealAmount);
            if (curPlayer.CurrentHealth > 0)
            {
                areRevived++;
            }
        }

        if (areRevived == gameManager.Players.Count)
        {
            for (int i = 0; i < gameManager.Enemies.Count; i++)
            {
                gameManager.Enemies[i].isGameOver = false;
            }
        }
        //Debug.Log("PlayersHealed: " + HealAmount);
    }
}
