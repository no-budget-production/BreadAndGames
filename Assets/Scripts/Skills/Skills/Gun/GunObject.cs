using System.Collections;
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

    public void LateSkillSetUp()
    {
        OverChargeThreshhold = Character.maxOverCharge;
    }

    public override void LateSkillSetup()
    {
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
            if (Character.curActionPoints > Character.curOverCharge)
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

        }
        else
        {
            if (isOverChargeDamageApplied)
            {
                Character.RangeDamageMultiplicator -= OverChargeDamageMultiplier;
                isOverChargeDamageApplied = false;
            }
        }
    }
}
