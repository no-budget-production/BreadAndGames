public class PlayerActionPoints : Cheat
{
    public float ActionPointsAmount;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public override void Shoot()
    {
        for (int i = 0; i < gameManager.Players.Count; i++)
        {
            PlayerController curPlayer = gameManager.Players[i].GetComponent<PlayerController>();

            curPlayer.RestoreActionPoints(ActionPointsAmount);
        }
        //Debug.Log("PlayersHealed: " + ActionPointsAmount);
    }
}
