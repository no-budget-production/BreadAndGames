using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlain : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        var tempPlayer = collision.gameObject.GetComponent<PlayerController>();
        if (tempPlayer != null)
        {
            if (tempPlayer.Type == PlayerType.Melee)
            {
                tempPlayer.transform.position = GameObject.Find("MeleeSpawnPoint").transform.position;
            }
            else if (tempPlayer.Type == PlayerType.Shooter)
            {
                tempPlayer.transform.position = GameObject.Find("ShooterSpawnPoint").transform.position;
            }
        }
    }
}
