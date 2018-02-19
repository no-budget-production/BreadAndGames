using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeShieldSkill : Skill
{
    public MeleeShield meleeShield;
    private MeleeShield cur_meleeShield;
    float nextShotTime;
    public float SecBetweenShot;
    private bool activ = false;

    public override void Shoot()
    {
        if (Time.time > nextShotTime && Character.curActionPoints > 0)
        {
            if (!activ)
            { 

                cur_meleeShield = Instantiate(meleeShield, Vector3.zero, Quaternion.identity);
                cur_meleeShield.transform.parent = transform;
                cur_meleeShield.transform.localRotation = Quaternion.identity;
                cur_meleeShield.transform.localPosition = Vector3.zero;
                activ = true;
            }
            else
            {
                Destroy(cur_meleeShield.gameObject);
                activ = false;
            }
            nextShotTime = Time.time + SecBetweenShot;
        }
    }
    void Update()
    {
        if (activ)
        {
            //Character.curActionPoints -= factor;
        }
    }
}
