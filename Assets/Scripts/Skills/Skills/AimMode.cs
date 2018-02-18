using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimMode : Skill
{
    public BuffObject buffObject;

    public override void Shoot()
    {
        PlayerController.AddBuff(buffObject, 1, PlayerController);
        //if (!base.PlayerController.HasBuff(buffObject))
        //{
        //    base.PlayerController.AddBuff(buffObject, 1);
        //    Debug.Log("Skill AddingBuff " + buffObject.name);
        //}


        //SkillEvents();

    }

    public override void StopShoot()
    {
        //if (base.PlayerController.HasBuff(buffObject))
        //{
        //    base.PlayerController.AddBuff(buffObject, -1);
        //    Debug.Log("Skill RemovingBuff " + buffObject.name);
        //}


    }

    //private void SkillEvents()
    //{
    //    base.SpawnBuff();

    //}
}
