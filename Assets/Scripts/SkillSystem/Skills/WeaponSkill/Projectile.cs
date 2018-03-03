﻿using System.Collections.Generic;
using UnityEngine;

public class Projectile : Effect
{
    public LayerMask collisionMask;

    public int EveryXFrames;
    private int FrameCounter;

    private int curPastFramesCounter;
    private Vector3[] PastFrameTransforms;

    public float MutliRayLength;

    public List<int> ReturnSelectedElements()
    {
        List<int> selectedElements = new List<int>();
        for (int i = 0; i < System.Enum.GetValues(typeof(UnitTypes)).Length; i++)
        {
            int layer = 1 << i;
            if (((int)ThisUnityTypeFlags & layer) != 0)
            {
                selectedElements.Add(i);
            }
        }
        return selectedElements;
    }

    [EnumFlagsAttribute]
    public UnitTypesFlags ThisUnityTypeFlags;

    public DamageType DamageType;

    public Character WeaponHolder;

    public float Speed = 10;
    public float Damage = 1;

    public float ProjectileMultiScaleFactor;

    private bool shieldImmunity = false;

    public ParticleSystem OnHit;

    private void Start()
    {
        PastFrameTransforms = new Vector3[EveryXFrames];

        for (int i = 0; i < PastFrameTransforms.Length; i++)
        {
            PastFrameTransforms[i] = transform.position;
        }
    }

    public void shieldImmunize()
    {
        shieldImmunity = true;
    }

    public void SetSpeed(float newSpeed)
    {
        Speed = newSpeed;

        StartCoroutine(Fade());
    }

    void Update()
    {
        float moveDistance = Speed * Time.deltaTime;

        FrameCounter++;
        if ((FrameCounter % EveryXFrames) == 0)
        {
            FrameCounter = 0;

            CheckCollisions(moveDistance);
        }

        transform.Translate(Vector3.forward * Time.deltaTime * Speed);

        PastFrameTransforms[FrameCounter] = transform.position;

    }

    void CheckCollisions(float moveDistance)
    {
        int LastFrame = 0;
        if (curPastFramesCounter - FrameCounter <= 0)
        {
            LastFrame = FrameCounter - curPastFramesCounter;
        }

        Ray ray = new Ray(PastFrameTransforms[LastFrame], transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance * EveryXFrames * MutliRayLength, collisionMask, QueryTriggerInteraction.Collide))
        {
            if (OnHitObject(hit))
                enabled = false;
            Destroy(gameObject, 5f);
        }
    }

    bool OnHitObject(RaycastHit hit)
    {
        MeleeShieldHandler MeleeShield = hit.collider.GetComponentInParent<MeleeShieldHandler>();
        if (MeleeShield != null && !shieldImmunity)
            return MeleeShield.AddProjectile(this);

        Entity damageableObject = hit.collider.GetComponent<Entity>();
        if (damageableObject == null)
            return true;

        var AllyOfWeaponHolder = WeaponHolder.NPC;
        var AllyOfVictim = hit.collider.GetComponent<Character>().NPC;

        if (AllyOfWeaponHolder == AllyOfVictim)
            return false;

        if (damageableObject.DiedAmount <= 0)
        {
            if (WeaponHolder != null)
            {
                var PlayerController = WeaponHolder.GetComponent<PlayerController>();
                if (PlayerController != null)
                {
                    float damageableObjectInitialHealth = damageableObject.CurrentHealth;

                    damageableObject.TakeDamage(WeaponHolder.RangeDamage * WeaponHolder.RangeDamageMultiplicator * Damage, DamageType);

                    if (damageableObject.CurrentHealth <= 0f)
                    {
                        StatsTracker.Instance.Kills[PlayerController.InternalPlayerNumber]++;

                        StatsTracker.Instance.DamageDealt[PlayerController.InternalPlayerNumber] += damageableObjectInitialHealth;
                    }
                    else
                    {
                        StatsTracker.Instance.DamageDealt[PlayerController.InternalPlayerNumber] += damageableObjectInitialHealth - damageableObject.CurrentHealth;
                    }
                }
                else
                {
                    damageableObject.TakeDamage(WeaponHolder.RangeDamage * WeaponHolder.RangeDamageMultiplicator * Damage, DamageType);
                }
            }
            else
            {
                damageableObject.TakeDamage(WeaponHolder.RangeDamage * WeaponHolder.RangeDamageMultiplicator * Damage, DamageType);
            }
        }

        if (OnHit != null)
        {
            ParticleSystem tempOnHit = Instantiate(OnHit, hit.transform.position, hit.transform.rotation);
            tempOnHit.transform.position = transform.position;
            tempOnHit.transform.rotation = transform.rotation;
            tempOnHit.Play();
            Destroy(tempOnHit.gameObject, 0.5f);
        }
        return true;
    }
}