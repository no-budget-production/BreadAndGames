using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeShieldHandler : MonoBehaviour
{
    public Transform meleeShield_Prefabs;
    private Transform meleeShield;
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

        meleeShield = Instantiate(meleeShield_Prefabs, Vector3.zero, Quaternion.identity).transform;
        meleeShield.parent = transform;
        meleeShield.localRotation = Quaternion.identity;
        meleeShield.localPosition = Vector3.zero;
        meleeShield.gameObject.SetActive(false);
        shootFrom = meleeShield.GetComponentInChildren<ShootFrom>().transform;
        
        //ShieldLogic
        damageMulti = 1;
    }

    public void Active()
    {
        if (Character.curActionPoints <= 0)
            return;
        if (IsActive())
            return;

        meleeShield.gameObject.SetActive(true);
    }

    public void Deactive()
    {
        if (!IsActive())
            return;

        meleeShield.gameObject.SetActive(false);
    }


    private bool IsActive()
    {
        return meleeShield.gameObject.activeSelf;
    }

    void Update()
    {
        if (!IsActive())
            return;

        meleeShield.forward = Character.lookVector;

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
            p.Damage *= damageMulti;
            p.transform.forward = Character.lookVector;
            p.transform.position = shootFrom.position;
            p.shieldImmunize();
            return false;
        }
        else
        {
            p.transform.forward = Character.lookVector;
            p.transform.position = shootFrom.position;
            damageMulti += damageMultiIncrease;
            return false;
        }
    }
}
