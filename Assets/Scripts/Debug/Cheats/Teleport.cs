using UnityEngine;

public class Teleport : Cheat
{
    public string MeleePort = "MeleePort";
    public string ShooterPort = "ShooterPort";

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    public override void Shoot()
    {
        gameManager.Players[0].GetComponent<Transform>().transform.position = GameObject.Find(MeleePort).transform.position;
        gameManager.Players[1].GetComponent<Transform>().transform.position = GameObject.Find(ShooterPort).transform.position;

        gameManager.Players[0].rb.velocity = Vector3.zero;
        gameManager.Players[1].rb.velocity = Vector3.zero;
    }
}