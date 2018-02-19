using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : Skill
{
    public DamageType DamageType;
    public PunchCollider HitBox;

    public float Damage;
    public BuffObject Debuff;

    public bool canCharge;
    public float ChargeTime;
    public float curChargeTime;
    public float BonusDamagePerSec;

    public SoundPlayer SoundPlayer;
    private SoundPlayer curSoundPlayer;
    private float nextSoundTime;
    public float SBetweenSounds;

    public override void LateSkillSetup()
    {
        transform.SetParent(SkillSpawn);
        curSoundPlayer = Instantiate(SoundPlayer, Character.transform.position + SoundPlayer.transform.position, Quaternion.identity);
        curSoundPlayer.transform.SetParent(SkillSpawn);
        curSoundPlayer.Play();
    }

    public override void Shoot()
    {

        if (BuffObject.HasBuff(Character.ActiveBuffObjects))
        {
            return;
        }

        if (BuffObject.HasCanTriggerWith(Character.ActiveBuffObjects))
        {
            return;
        }

        Character.AddBuff(BuffObject, 1, Character);

        if (canCharge)
        {
            curChargeTime += Time.deltaTime;

            if (ChargeTime < curChargeTime)
            {
                DeadlDamage();

                curChargeTime = 0;

                Debug.Log("Punch");
            }
        }
        else
        {
            DeadlDamage();
        }

    }

    public override void StopShoot()
    {
        DeadlDamage();

        curChargeTime = 0;

        Debug.Log("Punch Early");
    }


    public void DeadlDamage()
    {
        if (Time.time > nextSoundTime)
        {
            nextSoundTime = Time.time + SBetweenSounds + curSoundPlayer.GetClipLenght();
            curSoundPlayer.Play();
        }

        for (int i = 0; i < HitBox.Enemies.Count; i++)
        {
            if (HitBox.Enemies[i] == null)
            {
                HitBox.Enemies.Remove(HitBox.Enemies[i]);
                i--;
                continue;
            }

            if (Debuff != null)
            {
                var temp = HitBox.Enemies[i].GetComponent<Character>();
                if (temp != null)
                {
                    temp.AddBuff(Debuff, 1, Character);
                }
            }

            HitBox.Enemies[i].TakeDamage(Character.MeleeDamage * Character.MeleeDamageMultiplicator * (Damage + (curChargeTime * BonusDamagePerSec)), DamageType);
        }
    }
}
