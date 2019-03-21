﻿using System.Collections;
using UnityEngine;

public class WeaponSkill : Skill
{
    public Skill SkillRequired;

    public Transform Muzzle;
    public Transform ShellEjection;

    public Projectile Projectile;
    public Transform Shell;

    public float MuzzleVelocity = 35;
    public float AccuracyHorizontal;
    public float AccuracyVertical;

    public int ActionPointsCost;

    public int VolleySize;

    public MuzzleFlash MuzzleFlash;

    public SoundPlayer SoundPlayer;
    private float nextSoundTime;
    public float SBetweenSounds = 100;

    public bool isGunFound;

    public bool WaitForDelay;
    public float Delay;
    public float ReloadTime = 5;
    public bool WaitForShotAnim;
    public float SpendReloadAmount = 1 / 3;

    public bool isReloadingFully = true;
    public float ReloadActionPointsAmount = 100;


    public float ProjectileMultiScaleFactor;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public override void LateSkillSetup()
    {
        transform.SetParent(SkillSpawn);
    }

    //public void FindGunObject()
    //{
    //    int atI = 0;

    //    for (int i = 0; i < Character.ActiveSkills.Length; i++)
    //    {
    //        if (Character.ActiveSkills[i].SkillType == SkillRequired.SkillType)
    //        {
    //            //Debug.Log("Gun Found: " + Character.ActiveBuffs[i]);
    //            isGunFound = true;
    //            atI = i;
    //            break;
    //        }
    //    }
    //}

    public override void Shoot()
    {
        if (!BuffObject.isStackable)
        {
            if (BuffObject.HasBuff(Character.ActiveBuffObjects))
            {
                return;
            }
        }

        if (BuffObject.HasCanTriggerWith(Character.ActiveBuffObjects))
        {
            return;
        }

        Character.AddBuff(BuffObject, 1, Character);

        if (Character.curActionPoints - ActionPointsCost > 0)
        {
            if (Time.time > nextSoundTime)
            {
                nextSoundTime = Time.time + SBetweenSounds + SoundPlayer.GetClipLenght();
                //SoundPlayer.Play();
            }

            if (AnimationStrings[0] != null)
            {
                if (AnimationTypes[0] == AnimTypes.Trigger)
                {
                    Character._Animtor.SetTrigger(AnimationStrings[0]);
                }
            }

            if (!WaitForDelay)
            {
                if (!WaitForShotAnim)
                {
                    SpawnProjectiles();
                }
            }
            else
            {
                StopCoroutine("DelayWait");
                StartCoroutine(DelayWait());
            }
        }
        if (Character.curActionPoints < ActionPointsCost)
        {
            Character.EmptySound();
        }
    }

    void SpawnProjectiles()
    {
        //Debug.Log("ShotFired " + Character.name + " " + Time.realtimeSinceStartup);

        float AccuracyBonus = Mathf.Max(Character.AccuracyMultiplicator, 0.0001f);

        float tempAccuracyHorizontal = Mathf.Max(AccuracyHorizontal * AccuracyBonus, 0);
        float tempAccuracyVertical = Mathf.Max(AccuracyVertical * AccuracyBonus, 0);
        tempAccuracyHorizontal = Mathf.Min(tempAccuracyHorizontal, 180);
        tempAccuracyVertical = Mathf.Min(tempAccuracyVertical, 180);

        for (int i = 0; i < VolleySize; i++)
        {
            Quaternion accuracy = Quaternion.Euler(Random.Range(-tempAccuracyHorizontal, tempAccuracyHorizontal), Random.Range(-tempAccuracyVertical, tempAccuracyVertical), 0);

            Projectile newProjectile = Instantiate(Projectile, Muzzle.position, Muzzle.rotation * accuracy) as Projectile;
            newProjectile.WeaponHolder = Character;
            newProjectile.SetSpeed(MuzzleVelocity);
            newProjectile.transform.SetParent(gameManager.ProjectileHolder);

            if (Character.RangeDamageMultiplicator != 1f)
            {
                var trailRenderer = newProjectile.GetComponent<TrailRenderer>();
                if (trailRenderer != null)
                {

                    trailRenderer.startWidth *= (Character.RangeDamageMultiplicator * ProjectileMultiScaleFactor);
                }
            }

        }

        if (Shell != null)
        {
            Transform newShell = Instantiate(Shell, ShellEjection.position, ShellEjection.rotation);
            newShell.transform.SetParent(gameManager.VisualsHolder);
        }

        MuzzleFlash.Activate();

        Character.SpendActionPoints(ActionPointsCost);
    }

    public override void SkillHit()
    {
        SpawnProjectiles();
    }

    public virtual IEnumerator DelayWait()
    {

        yield return new WaitForSeconds(Delay);

        SpawnProjectiles();

        yield return null;

    }
}
