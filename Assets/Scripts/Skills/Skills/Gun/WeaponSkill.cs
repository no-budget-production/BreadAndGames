﻿using System.Collections;
using System.Collections.Generic;
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
    private SoundPlayer curSoundPlayer;
    private float nextSoundTime;
    public float SBetweenSounds = 100;

    public bool isGunFound;

    public override void LateSkillSetup()
    {
        this.transform.SetParent(SkillSpawn);
        curSoundPlayer = Instantiate(SoundPlayer, Character.transform.position + SoundPlayer.transform.position, Quaternion.identity);
        curSoundPlayer.transform.SetParent(SkillSpawn);
        curSoundPlayer.Play();
    }

    public void FindGunObject()
    {
        int atI = 0;

        for (int i = 0; i < Character.ActiveSkills.Length; i++)
        {
            if (Character.ActiveSkills[i].SkillType == SkillRequired.SkillType)
            {
                Debug.Log("Gun Found: " + Character.ActiveBuffs[i]);
                isGunFound = true;
                atI = i;
                break;
            }
        }
    }

    public override void Shoot()
    {
        if (BuffObject.HasBuff(Character.ActiveBuffObjects))
        {
            return;
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
                nextSoundTime = Time.time + SBetweenSounds + curSoundPlayer.GetClipLenght();
                curSoundPlayer.Play();
            }

            float AccuracyBonus = Mathf.Max(Character.AccuracyMultiplicator, 0.0001f);

            float tempAccuracyHorizontal = Mathf.Max(AccuracyHorizontal * AccuracyBonus, 0);
            float tempAccuracyVertical = Mathf.Max(AccuracyVertical * AccuracyBonus, 0);
            tempAccuracyHorizontal = Mathf.Min(tempAccuracyHorizontal, 180);
            tempAccuracyVertical = Mathf.Min(tempAccuracyVertical, 180);

            for (int i = 0; i < VolleySize; i++)
            {
                Quaternion accuracy = Quaternion.Euler(Random.Range(-tempAccuracyHorizontal, tempAccuracyHorizontal), Random.Range(-tempAccuracyVertical, tempAccuracyVertical), 0);

                Projectile newProjectile = Instantiate(Projectile, Muzzle.position, Muzzle.rotation * accuracy) as Projectile;
                newProjectile.Shooter = Character;
                newProjectile.SetSpeed(MuzzleVelocity);
                newProjectile.transform.SetParent(GameManager.Instance.ProjectileHolder);
            }

            if (Shell != null)
            {
                Transform newShell = Instantiate(Shell, ShellEjection.position, ShellEjection.rotation);
                newShell.transform.SetParent(GameManager.Instance.VisualsHolder);
            }

            MuzzleFlash.Activate();

            Character.SpendActionPoints(ActionPointsCost);
        }
        else if (Character.curActionPoints <= 0)
        {
            Character.EmptySound();
        }
    }
}