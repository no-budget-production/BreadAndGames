using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : Skill
{

    public PunchCollider hitBoxSweep;
    public PunchCollider hitBoxPunch;

    public float MsBetweenShot;
    public float Damage;

    float nextShotTime;

    public override void Shoot()
    {
        if (Time.time > nextShotTime && base.PlayerController.curActionPoints > 0 && !base.PlayerController.isInAction)
        {
           Debug.Log("Punch");
           hitBoxSweep.enemies.ForEach(e => e.TakeDamage(Damage));
           hitBoxPunch.enemies.ForEach(e => e.TakeDamage(Damage));
        }
    }
}
