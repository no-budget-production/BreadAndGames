using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeShieldSkill : Skill
{
    public MeleeShield meleeShield;
    float nextShotTime;
    public float MsBetweenShot;
    public float cooldown = 6;
    private float cur_cooldown = 0;
    public float stayActivTime = 3;

    public override void Shoot()
    {
        if (cur_cooldown < Time.time)
        {
            if (Time.time > nextShotTime && Character.curActionPoints > 0)
            {
                nextShotTime = Time.time + MsBetweenShot * 0.001f;

                var cur_meleeShield = Instantiate(meleeShield, Vector3.zero, Quaternion.identity);
                cur_meleeShield.transform.parent = transform;
                cur_meleeShield.transform.localRotation = Quaternion.identity;
                cur_meleeShield.transform.localPosition = Vector3.zero;

                Destroy(cur_meleeShield.gameObject, stayActivTime);
                cur_cooldown = cooldown + Time.time;
            }
        }
    }
    void Update()
    {
        //if (cur_cooldown )
    }
}
