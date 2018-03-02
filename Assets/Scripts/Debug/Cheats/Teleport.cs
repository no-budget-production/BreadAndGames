using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : Cheat
{
    public string MeleePort = "MeleePort";
    public string ShooterPort = "ShooterPort";

    public override void Shoot()
    {
        GameManager.Instance.Players[0].GetComponent<Transform>().transform.position = GameObject.Find(MeleePort).transform.position;
        GameManager.Instance.Players[1].GetComponent<Transform>().transform.position = GameObject.Find(ShooterPort).transform.position;

        GameManager.Instance.Players[0].rb.velocity = Vector3.zero;
        GameManager.Instance.Players[1].rb.velocity = Vector3.zero;
    }
}