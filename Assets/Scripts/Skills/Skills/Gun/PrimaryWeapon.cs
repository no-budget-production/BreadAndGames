using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryWeapon : Skill
{
    public Skill SkillRequired;

    public Transform Muzzle;
    public Transform ShellEjection;

    public Projectile Projectile;
    public Transform Shell;

    public float MsBetweenShot = 100;
    public float MuzzleVelocity = 35;
    public float AccuracyHorizontal;
    public float AccuracyVertical;

    public int ActionPointsCost;

    public int VolleySize;

    public MuzzleFlash MuzzleFlash;

    private float nextShotTime;

    public SoundPlayer SoundPlayer;
    private SoundPlayer curSoundPlayer;
    private float nextSoundTime;
    public float SBetweenSounds = 100;

    public bool isGunFound;

    public override void LateSkillSetup()
    {
        this.transform.SetParent(SkillSpawn);
        curSoundPlayer = Instantiate(SoundPlayer, PlayerController.transform.position + SoundPlayer.transform.position, Quaternion.identity);
        curSoundPlayer.transform.SetParent(SkillSpawn);
        curSoundPlayer.Play();
    }

    public void FindGunObject()
    {
        int atI = 0;

        for (int i = 0; i < PlayerController.ActiveSkills.Length; i++)
        {
            if (PlayerController.ActiveSkills[i].SkillType == SkillRequired.SkillType)
            {
                Debug.Log("Gun Found: " + PlayerController.ActiveBuffs[i]);
                isGunFound = true;
                atI = i;
                break;
            }
        }
    }

    public override void Shoot()
    {
        if (BuffObject.HasBuff(PlayerController.ActiveBuffObjects))
        {
            return;
        }

        if (BuffObject.HasCanTriggerWith(PlayerController.ActiveBuffObjects))
        {
            return;
        }

        PlayerController.AddBuff(BuffObject, 1, PlayerController);

        if (PlayerController.curActionPoints > 0)
        {
            if (Time.time > nextSoundTime)
            {
                nextSoundTime = Time.time + SBetweenSounds + curSoundPlayer.GetClipLenght();
                curSoundPlayer.Play();
            }

            float AccuracyBonus = Mathf.Max(base.PlayerController.AccuracyMultiplicator, 0.0001f);

            float tempAccuracyHorizontal = Mathf.Max(AccuracyHorizontal * AccuracyBonus, 0);
            float tempAccuracyVertical = Mathf.Max(AccuracyVertical * AccuracyBonus, 0);
            tempAccuracyHorizontal = Mathf.Min(tempAccuracyHorizontal, 180);
            tempAccuracyVertical = Mathf.Min(tempAccuracyVertical, 180);

            for (int i = 0; i < VolleySize; i++)
            {
                Quaternion accuracy = Quaternion.Euler(Random.Range(-tempAccuracyHorizontal, tempAccuracyHorizontal), Random.Range(-tempAccuracyVertical, tempAccuracyVertical), 0);

                Projectile newProjectile = Instantiate(Projectile, Muzzle.position, Muzzle.rotation * accuracy) as Projectile;
                newProjectile.Shooter = base.PlayerController;
                newProjectile.SetSpeed(MuzzleVelocity);
            }

            Instantiate(Shell, ShellEjection.position, ShellEjection.rotation);

            MuzzleFlash.Activate();

            base.PlayerController.SpendActionPoints(ActionPointsCost);
        }
        else if (base.PlayerController.curActionPoints <= 0)
        {
            PlayerController.EmptySound();
        }
    }
}
