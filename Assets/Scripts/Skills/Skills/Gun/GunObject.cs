using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunObject : Skill
{
    public Transform Muzzle;
    public Transform ShellEjection;

    public Laser LaserScript;

    public float ActionPointsPerSec;

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
    }
}
