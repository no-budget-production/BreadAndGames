public class Player1 : Cheat
{
    public PlayerController NewPlayer1;
    public PlayerType TargetType;
    bool isPlayerFound;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public override void Shoot()
    {
        if (NewPlayer1 == null)
        {
            NewPlayer1 = gameManager.GetPlayerByType(TargetType);
        }

        int j = 2;

        for (int i = 0; i < gameManager.Players.Count; i++)
        {
            PlayerController curPlayer = gameManager.Players[i].GetComponent<PlayerController>();

            if (curPlayer != gameManager.GetPlayerByType(TargetType))
            {
                curPlayer.PlayerNumber = j.ToString();
                j++;
            }
            else
            {
                curPlayer.PlayerNumber = "1";
                isPlayerFound = true;
            }

            if (!isPlayerFound && (i == gameManager.Players.Count))
            {
                curPlayer.PlayerNumber = "1";
                ////Debug.Log(NewPlayer1.name + "not found!");
            }
            curPlayer.Setup(gameManager.InputRotation, gameManager.prefabLoader.ButtonStrings);
        }
        //if (NewPlayer1 != null)
        //{
        //    Debug.Log(NewPlayer1.name + "new Player 1");
        //}
    }
}
