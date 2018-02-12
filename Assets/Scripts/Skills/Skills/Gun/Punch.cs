using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : Skill
{

    public PunchCollider HitBox;

    public float MsBetweenShot;
    public float Damage;
    public float chargeTime;
    public float cur_chargeTime;

    float nextShotTime;
    void Start()
    {
        cur_chargeTime = chargeTime;
    }

    public override void Shoot()
    {
        if (cur_chargeTime < 100) cur_chargeTime -= Time.deltaTime;
        if (Time.time > nextShotTime && base.PlayerController.curActionPoints > 0 && !base.PlayerController.isInAction && cur_chargeTime <= 0)
        {
            nextShotTime = Time.time + MsBetweenShot * 0.001f;


            foreach (Character e in HitBox.Enemies)
            {
                if (e == null)
                {
                    HitBox.Enemies.Remove(e);
                    return;
                }
                e.TakeDamage(Damage);
            }

            cur_chargeTime = 110;
        }
    }

    public override void StopShoot()
    {
        cur_chargeTime = chargeTime;
    }
}
