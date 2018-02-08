using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : Skill
{

    public PunchCollider hitBox;

    public float MsBetweenShot;
    public float Damage;

    float nextShotTime;

    public override void Shoot()
    {
        if (Time.time > nextShotTime && base.PlayerController.curActionPoints > 0 && !base.PlayerController.isInAction)
        {
           Debug.Log("Punch");
            foreach (Character e in hitBox.enemies)
            {
                if (e == null)
                {
                    hitBox.enemies.Remove(e);
                    return;
                }
                e.TakeDamage(Damage);
            }
        }
    }
}
