using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimMode : Skill
{

    public override void Shoot()
    {
        SkillEvents();
    }

    private void SkillEvents()
    {
        base.SpawnBuff();

    }
}
