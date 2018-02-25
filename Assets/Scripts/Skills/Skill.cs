using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [Header(">>>>>>>>>> Skill:")]

    public SkillType SkillType;
    public int SkillNumber;

    [HideInInspector]
    public bool isFiring;
    public Transform SkillSpawn;
    public BuffObject BuffObject;

    public Buff UsedBuff;
    public Buff curBuff;
    public float BuffDuration;

    public bool cantStack;

    public bool HasAnimation;
    public string[] AnimationStrings;
    public AnimTypes[] AnimationTypes;

    [Header("<<<<<<<<<< Skill:")]

    [HideInInspector]
    public Character Character;

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

    public virtual void SkillHit()
    {

    }

    public virtual void SpawnBuff()
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
        curBuff.Lifetime2 = BuffDuration;
        curBuff.Skill = this;
        Character.ActiveBuffs.Add(curBuff);
    }
}
