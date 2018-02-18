using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunObject : Skill
{
    public Transform Muzzle;
    public Transform ShellEjection;

    public Laser LaserScript;

    public override void LateSkillSetup()
    {
        this.transform.SetParent(SkillSpawn);
    }
}
