using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : Skill
{

    public PunchCollider HitBox;

    public float MsBetweenShot;
    public float Damage;

    float nextShotTime;

    public override void Shoot()
    {
        if (Time.time > nextShotTime && base.PlayerController.curActionPoints > 0 && !base.PlayerController.isInAction)
        {
            nextShotTime = Time.time + MsBetweenShot * 0.001f;

            //Debug.Log("Punch");
            foreach (Character e in HitBox.Enemies)
            {
                if (e == null)
                {
                    HitBox.Enemies.Remove(e);
                    return;
                }
                e.TakeDamage(Damage);
            }
        }
    }
}
