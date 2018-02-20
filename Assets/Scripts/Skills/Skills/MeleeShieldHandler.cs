﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeShieldHandler : MonoBehaviour
{
    public Transform meleeShieldLevel1_Prefabs;
    public Transform meleeShieldLevel2_Prefabs;
    private Transform meleeShieldLevel1;
    private Transform meleeShieldLevel2;
    float nextShotTime;
    public float SecBetweenShot;

    //ShieldLogic
    public float damageMulti = 1;
    public float damageMultiIncrease = 0.1f;
    public float energyCosts;
    private PlayerController Character;
    private Transform shootFrom;

    void Start()
    {
        Character = GetComponentInParent<PlayerController>();

        meleeShieldLevel1 = Instantiate(meleeShieldLevel1_Prefabs, Vector3.zero, Quaternion.identity).transform;
        meleeShieldLevel1.parent = transform;
        meleeShieldLevel1.localRotation = Quaternion.identity;
        meleeShieldLevel1.localPosition = Vector3.zero;
        meleeShieldLevel1.gameObject.SetActive(false);

        meleeShieldLevel2 = Instantiate(meleeShieldLevel2_Prefabs, Vector3.zero, Quaternion.identity).transform;
        meleeShieldLevel2.parent = transform;
        meleeShieldLevel2.localRotation = Quaternion.identity;
        meleeShieldLevel2.localPosition = Vector3.zero;
        meleeShieldLevel2.gameObject.SetActive(false);
        shootFrom = meleeShieldLevel2.GetComponentInChildren<ShootFrom>().transform;
        //ShieldLogic
        damageMulti = 1;
    }

    public void Active()
    {
        if (Character.curActionPoints <= 0) return;
        if (IsActive()) return;

        meleeShieldLevel1.gameObject.SetActive(true);
    }

    public void Overcharge()
    {
        if (Character.curActionPoints <= 0) return;
        if (!IsActive()) return;

        if (!IsOvercharge())
        {
            meleeShieldLevel1.gameObject.SetActive(false);
            meleeShieldLevel2.gameObject.SetActive(true);
        }
        else
        {
            meleeShieldLevel1.gameObject.SetActive(true);
            meleeShieldLevel2.gameObject.SetActive(false);
        }
    }

    public void Deactive()
    {
        if (!IsActive()) return;

        meleeShieldLevel1.gameObject.SetActive(false);
        meleeShieldLevel2.gameObject.SetActive(false);
    }


    private bool IsActive()
    {
        return meleeShieldLevel1.gameObject.activeSelf || meleeShieldLevel2.gameObject.activeSelf;
    }
    private bool IsOvercharge()
    {
        return meleeShieldLevel2.gameObject.activeSelf;
    }

    void Update()
    {
        if (!IsActive()) return;

        meleeShieldLevel1.forward = Character.lookVector;
        meleeShieldLevel2.forward = Character.lookVector;
        
        Character.curActionPoints -= Time.deltaTime * energyCosts;
        if (Character.curActionPoints <= 0)
        {
            Deactive();
        }
    }

    //ShieldLogic
    public bool AddProjectile(Projectile p)
    {
        bool friendly = (p.Shooter as PlayerController) != null;
        if (friendly)
        {
            if (IsOvercharge())
            {
                p.Damage *= damageMulti;
                p.transform.forward = Character.lookVector;
                p.transform.position = shootFrom.position;
                p.shieldImmunize();
                return false;
            }
            else
            {
                //Character.curActionPoints += factor;
                return true;
            }
        }
        else
        {
            if (IsOvercharge())
            {
                damageMulti += damageMultiIncrease;
            }
            else
            {
                //Character.curActionPoints += factor;
            }
            return true;
        }
    }
}
