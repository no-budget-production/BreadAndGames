﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : Skill
{
    public DamageType DamageType;
    public PunchCollider HitBox;

    public float Damage;
    public BuffObject ChargingBuff;
    public BuffObject Debuff;

    public bool canCharge;
    public float ChargeTime;
    public float curChargeTime;
    public float BonusDamagePerSec;
    public float energyCosts;

    public int punchCount;

    public SoundPlayer SoundPlayer;
    private float nextSoundTime;
    public float SBetweenSounds;

    public float curDamageBonus;

    public override void LateSkillSetup()
    {
        transform.SetParent(SkillSpawn);
    }

    public override void Shoot()
    {
        if (canCharge)
        {
            if (Character.curActionPoints - energyCosts > 0)
            {
                //Debug.Log("CanCharge");
                if (!ChargingBuff.HasBuff(Character.ActiveBuffObjects))
                {
                    //Debug.Log("!ChargingBuff.HasBuff");
                    if (!BuffObject.isStackable)
                    {
                        //Debug.Log("!BuffObject.isStackable");
                        if (BuffObject.HasBuff(Character.ActiveBuffObjects))
                        {
                            //Debug.Log("BuffObject.HasBuff");
                            return;
                        }
                    }
                }
            }
            else
            {
                if (!BuffObject.isStackable)
                {
                    if (BuffObject.HasBuff(Character.ActiveBuffObjects))
                    {
                        return;
                    }
                }
            }
        }
        else
        {
            if (!BuffObject.isStackable)
            {
                if (BuffObject.HasBuff(Character.ActiveBuffObjects))
                {
                    return;
                }
            }
        }

        if (BuffObject.HasCanTriggerWith(Character.ActiveBuffObjects))
        {
            return;
        }

        Character.AddBuff(BuffObject, 1, Character);

        if (canCharge)
        {
            if (Character.curActionPoints - energyCosts > 0)
            {
                Character.AddBuff(ChargingBuff, 1, Character);

                //Debug.Log("Character.AddBuff");

                curChargeTime += Time.deltaTime;
                curDamageBonus += BonusDamagePerSec * Time.deltaTime;
                Character.SpendActionPoints(energyCosts * Time.deltaTime);
                //Debug.Log("TimeDeltaTime:" + Time.deltaTime);
                //Debug.Log("TimeDeltaTime:" + curChargeTime);
                //Character.SpendActionPoints(energyCosts * 1 + curChargeTime);

                if (ChargeTime < curChargeTime)
                {
                    punchCount++;
                    //Debug.Log("//////////////////////////////////////////////////////////////////////////punchCount " + punchCount);
                    DeadlDamage();
                    curChargeTime = 0;
                    curDamageBonus = 0;
                    Character.AddBuff(ChargingBuff, -1, Character);
                }
            }
            else
            {
                //Debug.Log("###############################################################NoEnerhyPuncch ");
                DeadlDamage();
                curChargeTime = 0;
                curDamageBonus = 0;
                Character.AddBuff(ChargingBuff, -1, Character);
            }
        }
        else
        {
            Character.SpendActionPoints(energyCosts);
            DeadlDamage();
            curChargeTime = 0;
            curDamageBonus = 0;
        }


        //Debug.Log("Energy " + energyCosts * Time.deltaTime);
        //Debug.Log("curDamageBonus: " + curDamageBonus + " Time: " + Time.realtimeSinceStartup);
    }

    public override void StopShoot()
    {
        if (canCharge)
        {
            //Debug.Log("CanCharge");
            if (!ChargingBuff.HasBuff(Character.ActiveBuffObjects))
            {
                //Debug.Log("!ChargingBuff.HasBuff");
                if (!BuffObject.isStackable)
                {
                    //Debug.Log("!BuffObject.isStackable");
                    if (BuffObject.HasBuff(Character.ActiveBuffObjects))
                    {
                        //Debug.Log("BuffObject.HasBuff");
                        return;
                    }
                }
            }
        }
        else
        {
            if (!BuffObject.isStackable)
            {
                if (BuffObject.HasBuff(Character.ActiveBuffObjects))
                {
                    return;
                }
            }
        }

        if (BuffObject.HasCanTriggerWith(Character.ActiveBuffObjects))
        {
            return;
        }

        Character.AddBuff(BuffObject, 1, Character);

        DeadlDamage();

        curChargeTime = 0;

        if (canCharge)
        {
            Character.AddBuff(ChargingBuff, -1, Character);
        }

        //Debug.Log("###############################################################QUICKPUNCH -------------------------------- ");

        //Debug.Log("Punch Early");
    }


    public void DeadlDamage()
    {
        if (Time.time > nextSoundTime)
        {
            nextSoundTime = Time.time + SBetweenSounds + SoundPlayer.GetClipLenght();
            //SoundPlayer.Play();
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

            HitBox.Enemies[i].TakeDamage(Character.MeleeDamage * Character.MeleeDamageMultiplicator * (Damage + (curDamageBonus)), DamageType);
        }

        Character.curOverCharge = Character.curActionPoints;
        Character.OnChangeOverchargeSlider();


    }
}
