using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimMode : Skill
{
    public override void Shoot()
    {
        PlayerController.AddBuff(BuffObject, 1, PlayerController);
    }

    public override void StopShoot()
    {

    }

}
