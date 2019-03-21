public class KillAllEnemies : Cheat
{
    public float TakeDamage;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public override void Shoot()
    {
        for (int i = 0; i < GameManager.Instance.Enemies.Count; i++)
        {
            GameManager.Instance.Enemies[i].TakeDamage(TakeDamage, DamageType.Melee);

            i--;
        }
    }
}
