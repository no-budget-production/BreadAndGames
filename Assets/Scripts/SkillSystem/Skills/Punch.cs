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

    public SoundPlayer SoundPlayer;
    private float nextSoundTime;
    public float SBetweenSounds;

    public float curDamageBonus;

    public float minEnergyCost;
    private float currentEneryCost;

    private StatsTracker statsTracker;

    private void Start()
    {
        statsTracker = StatsTracker.Instance;
    }

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
            //Character.canWalk = false;

            Character.AddBuff(ChargingBuff, 1, Character);

            if (Character.curActionPoints - energyChargeCosts * Time.deltaTime > 0)
            {
                if (AnimationStrings[0] != null)
                {
                    if (AnimationTypes[0] == AnimTypes.Bool)
                    {
                        Character._Animtor.SetBool(AnimationStrings[0], true);
                    }
                }

                curChargeTime += Time.deltaTime;
                float thisFrameEnergyCosts = energyChargeCosts * Time.deltaTime;
                currentEneryCost += thisFrameEnergyCosts;
                curDamageBonus += BonusDamagePerSec * Time.deltaTime;

                if (currentEneryCost >= minEnergyCost)
                {
                    Character.SpendActionPoints(thisFrameEnergyCosts);
                }

                if (ChargeTime < curChargeTime)
                {
                    DeadlDamage();
                    Character.AddBuff(ChargingBuff, -1, Character);
                }
            }
            else
            {
                if (AnimationStrings[1] != null)
                {
                    if (AnimationTypes[1] == AnimTypes.Trigger)
                    {
                        Character._Animtor.SetTrigger(AnimationStrings[1]);
                    }
                }
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
    }

    public override void StopShoot()
    {
        if (canCharge)
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
        }

        Character.AddBuff(BuffObject, 1, Character);

        DeadlDamage();

        curChargeTime = 0;
    }


    public void DeadlDamage()
    {
        if (Time.time > nextSoundTime)
        {
            nextSoundTime = Time.time + SBetweenSounds + SoundPlayer.GetClipLenght();
        }

        Character.AddBuff(BuffObject, 1, Character);

        Character.curOverCharge = Character.curActionPoints;
        Character.OnChangeOverchargeSlider();

        if (canCharge)
        {
            Character._Animtor.SetBool(AnimationStrings[0], false);
        }

        curChargeTime = 0;

        currentEneryCost = 0;
    }

    public override void SkillHit()
    {
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
            if (HitBox.Enemies[i].DiedAmount <= 0)
            {
                if (Character != null)
                {
                    var PlayerController = Character.GetComponent<PlayerController>();
                    if (PlayerController != null)
                    {
                        float damageableObjectInitialHealth = HitBox.Enemies[i].CurrentHealth;

                        HitBox.Enemies[i].TakeDamage(Character.MeleeDamage * Character.MeleeDamageMultiplicator * (Damage + (curDamageBonus)), DamageType);

                        if (HitBox.Enemies[i].CurrentHealth <= 0f)
                        {
                            statsTracker.Kills[PlayerController.InternalPlayerNumber]++;

                            statsTracker.DamageDealt[PlayerController.InternalPlayerNumber] += damageableObjectInitialHealth;
                        }
                        else
                        {
                            statsTracker.DamageDealt[PlayerController.InternalPlayerNumber] += damageableObjectInitialHealth - HitBox.Enemies[i].CurrentHealth;

                        }
                    }
                    else
                    {
                        HitBox.Enemies[i].TakeDamage(Character.MeleeDamage * Character.MeleeDamageMultiplicator * (Damage + (curDamageBonus)), DamageType);
                    }
                }
                //else
                //{
                //    HitBox.Enemies[i].TakeDamage(Character.MeleeDamage * Character.MeleeDamageMultiplicator * (Damage + (curDamageBonus)), DamageType);
                //}
            }

            if (HitBox.Enemies[i].CurrentHealth <= 0)
            {
                AddStats();
            }
        }

        if (canCharge)
        {
            Character.AddBuff(ChargingBuff, -1, Character);
        }

        curDamageBonus = 0;
    }

    public void AddStats()
    {
        var PlayerController = Character.GetComponent<PlayerController>();
        if (PlayerController != null)
        {
            statsTracker.Kills[PlayerController.InternalPlayerNumber]++;
        }
    }
}
