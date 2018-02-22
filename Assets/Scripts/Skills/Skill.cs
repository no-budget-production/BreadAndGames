using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public SkillType SkillType;

    public Character Character;

    public bool isFiring;
    public Transform SkillSpawn;

    public BuffObject BuffObject;

    public Buff UsedBuff;
    public Buff curBuff;
    public float BuffDuration;

    public bool cantStack;

    public virtual void OneShoot()
    {

    }

    public virtual void Shoot()
    {

    }

    public virtual void StopShoot()
    {

    }

    public virtual void LateSkillSetup()
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

            for (int i = 0; i < Character.ActiveBuffs.Count; i++)
            {
                if (Character.ActiveBuffs[i].BuffSkillType == UsedBuff.BuffSkillType)
                {
                    Debug.Log("Spawn Buff found: " + Character.ActiveBuffs[i]);
                    Destroy(Character.ActiveBuffs[i].gameObject);
                    Character.ActiveBuffs.Remove(Character.ActiveBuffs[i]);
                    break;
                }
            }
        }

        curBuff = Instantiate(UsedBuff, transform.position + UsedBuff.transform.position, Quaternion.identity);
        curBuff.transform.SetParent(transform);
        curBuff.Character = Character;
        curBuff.Lifetime = BuffDuration;
        Character.ActiveBuffs.Add(curBuff);
    }
}
