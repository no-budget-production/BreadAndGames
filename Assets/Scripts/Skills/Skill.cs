using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public int SkillID;

    public PlayerController PlayerController;

    public bool isFiring;
    public Transform SkillSpawn;

    public BuffObject BuffObject;

    public Buff UsedBuff;
    public Buff curBuff;
    public float BuffDuration;

    public bool cantStack;

    public virtual void Shoot()
    {

    }

    public virtual void StopShoot()
    {

    }

    public void SpawnBuff()
    {
        if (UsedBuff == null)
        {
            return;
        }

        if (cantStack)
        {
            Debug.Log("CantStack");

            for (int i = 0; i < PlayerController.ActiveBuffs.Count; i++)
            {
                if (PlayerController.ActiveBuffs[i].BuffID == UsedBuff.BuffID)
                {
                    Debug.Log("Spawn Buff found: " + PlayerController.ActiveBuffs[i]);
                    Destroy(PlayerController.ActiveBuffs[i].gameObject);
                    PlayerController.ActiveBuffs.Remove(PlayerController.ActiveBuffs[i]);
                    break;
                }
            }
        }

        curBuff = Instantiate(UsedBuff, transform.position + UsedBuff.transform.position, Quaternion.identity);
        curBuff.transform.SetParent(transform);
        curBuff.PlayerController = PlayerController;
        curBuff.Lifetime = BuffDuration;
        PlayerController.ActiveBuffs.Add(curBuff);
    }
}
