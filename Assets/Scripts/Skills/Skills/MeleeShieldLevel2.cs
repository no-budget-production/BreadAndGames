using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeShieldLevel2 : MonoBehaviour
{
    public float damageMulti;
    public float damageMultiIncrease = 0.1f;

    // Use this for initialization
    void Start()
    {
        damageMulti = 1;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool AddProjectile(Projectile p)
    {
        bool friendly = (p.Shooter as PlayerController) != null;
        if (friendly)
        {
            p.Damage *= damageMulti;
            p.transform.forward = transform.forward;
            p.shieldImmunize();
            return false;
        }
        else
        {
            damageMulti += damageMultiIncrease;
            return true;
        }
    }
}
