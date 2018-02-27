using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : Skill
{
    public float energyCosts;
    public float energyChargeCosts;

    public DamageType DamageType;
    public PunchCollider HitBox;

    public float Damage;
    public BuffObject ChargingBuff;
    public BuffObject Debuff;

    public bool canCharge;
    public float ChargeTime;
    public float curChargeTime;
    public float BonusDamagePerSec;


    public int punchCount;

    public SoundPlayer SoundPlayer;
    private float nextSoundTime;
    public float SBetweenSounds;

    public float curDamageBonus;

    public float minEnergyCost;
    private float currentEneryCost;

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
                if (!ChargingBuff.HasBuff(Character.ActiveBuffObjects))
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


        if (canCharge)
        {
            Debug.Log("!! CanWalk");

            Character.canWalk = false;

            Character.AddBuff(ChargingBuff, 1, Character);

            if (AnimationStrings[0] != null)
            {
                if (AnimationTypes[0] == AnimTypes.Bool)
                {
                    Character._Animtor.SetBool(AnimationStrings[0], true);
                }
            }

            if (Character.curActionPoints - energyChargeCosts > 0)
            {
                //Debug.Log("Character.AddBuff");

                curChargeTime += Time.deltaTime;
                float thisFrameEnergyCosts = energyChargeCosts * Time.deltaTime;
                currentEneryCost += thisFrameEnergyCosts;
                curDamageBonus += BonusDamagePerSec * Time.deltaTime;
                Debug.Log(curDamageBonus);
                if (currentEneryCost >= minEnergyCost)
                {
                    Character.SpendActionPoints(thisFrameEnergyCosts);
                }
                //Debug.Log("BonusDamagePerSec * Time.deltaTime:" + BonusDamagePerSec * Time.deltaTime);
                //Debug.Log("TimeDeltaTime:" + curChargeTime);

                if (ChargeTime < curChargeTime)
                {
                    punchCount++;
                    //Debug.Log("//////////////////////////////////////////////////////////////////////////punchCount " + punchCount);
                    DeadlDamage();
                    Character.AddBuff(ChargingBuff, -1, Character);
                }
            }
            else
            {
                //Debug.Log("###############################################################NoEnerhyPuncch ");
                DeadlDamage();
                Character.AddBuff(ChargingBuff, -1, Character);
            }
        }
        else
        {
            if (AnimationStrings[0] != null)
            {
                if (AnimationTypes[0] == AnimTypes.Trigger)
                {
                    Character._Animtor.SetTrigger(AnimationStrings[0]);
                }
            }
            Character.SpendActionPoints(energyCosts);
            DeadlDamage();
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

        if (canCharge)
        {
            Character._Animtor.SetBool(AnimationStrings[0], false);
            //Debug.Log("ResetBool");

        }

        Character.AddBuff(BuffObject, 1, Character);

        DeadlDamage();

        curChargeTime = 0;



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

        Character.AddBuff(BuffObject, 1, Character);

        Character.curOverCharge = Character.curActionPoints;
        Character.OnChangeOverchargeSlider();

        //Debug.Log("ResetBool?!?");

        if (canCharge)
        {
            Character._Animtor.SetBool(AnimationStrings[0], false);
            //Debug.Log("ResetBool");

        }

        curChargeTime = 0;

        currentEneryCost = 0;
    }

    public override void SkillHit()
    {
        //Debug.Log("Skill Hit: " + gameObject.name);

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

        if (canCharge)
        {
            Character.AddBuff(ChargingBuff, -1, Character);
        }

        curDamageBonus = 0;
    }

}
