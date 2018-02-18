using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimMode : Skill
{
    public override void Shoot()
    {
        Character.AddBuff(BuffObject, 1, Character);
    }

    public override void StopShoot()
    {

    }

}
