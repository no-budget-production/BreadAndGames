﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunObject : Skill
{
    public Transform Muzzle;
    public Transform ShellEjection;

    public Laser LaserScript;

    public float ActionPointsPerSec;

    public float OverChargeThreshhold;
    public float OverChargeDamageMultiplier;
    public bool isOverChargeDamageApplied;

    public override void LateSkillSetup()
    {
        OverChargeThreshhold = Character.curOverCharge;
        transform.SetParent(SkillSpawn);
        if (Character.UseOverChargeBar)
        {
            Character.OnChangeOverchargeSlider();
        }
    }

    private void Update()
    {
        if (Character.UseOverChargeBar)
        {
            if (Character.curActionPoints >= Character.curOverCharge)
            {
                Character.RestoreActionPoints(ActionPointsPerSec * Time.deltaTime);
            }
        }

        if (Character.curActionPoints < OverChargeThreshhold)
        {
            if (!isOverChargeDamageApplied)
            {
                Character.RangeDamageMultiplicator += OverChargeDamageMultiplier;
                isOverChargeDamageApplied = true;
            }

            Debug.Log("ExtraDamage");

        }
        else
        {
            if (isOverChargeDamageApplied)
            {
                Character.RangeDamageMultiplicator -= OverChargeDamageMultiplier;
                isOverChargeDamageApplied = false;
            }

            Debug.Log("NoExtraDamage");
        }
    }

    //Reload->

    public SoundPlayer SoundPlayer;
    private float nextSoundTime;
    public float SBetweenSounds = 100;

    public float ReloadActionPointsAmount;
    public float SpendReloadAmount;

    public bool isReloadingFully;

    public bool enableIfOverCharged;


    public override void Shoot()
    {
        if (!BuffObject.isStackable)
        {
            if (BuffObject.HasBuff(Character.ActiveBuffObjects))
            {
                return;
            }
        }

        if (BuffObject.HasCanTriggerWith(Character.ActiveBuffObjects))
        {
            return;
        }

        if (Character.curActionPoints >= Character.maxActionPoints)
        {
            return;
        }

        if (!Character.rechargeActionBarDirectly)
        {
            if (Character.curReloadBar < SpendReloadAmount)
            {
                return;
            }
        }

        if (enableIfOverCharged)
        {
            if (Character.curActionPoints > OverChargeThreshhold)
            {
                return;
            }
        }

        if (Time.time > nextSoundTime)
        {
            nextSoundTime = Time.time + SBetweenSounds + SoundPlayer.GetClipLenght();
            SoundPlayer.Play();
        }

        Character.AddBuff(BuffObject, 1, Character);

        if (isReloadingFully)
        {
            Character.RestoreActionPoints(Character.maxActionPoints);
        }
        else
        {
            Character.RestoreActionPoints(ReloadActionPointsAmount);
        }

        Character.SpendReloads(SpendReloadAmount);

        SoundPlayer.Play();
    }
}
