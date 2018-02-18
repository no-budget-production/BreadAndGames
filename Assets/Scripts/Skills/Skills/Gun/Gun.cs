using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Skill
{
    public Transform Muzzle;
    public Projectile Projectile;
    public float MsBetweenShot = 100;
    public float MuzzleVelocity = 35;
    public float AccuracyHorizontal;
    public float AccuracyVertical;

    public int ActionPointsCost;

    public int VolleySize;

    public MuzzleFlash MuzzleFlash;
    public AudioSource GunSound;
    public AudioClip[] SoundClips;
    public AudioClip SoundEmpty;

    public Transform Shell;
    public Transform ShellEjection;

    private float nextShotTime;

    public SoundPlayer SoundPlayer;
    private SoundPlayer curSoundPlayer;
    private float nextSoundTime;
    public float SBetweenSounds = 100;

    private void Start()
    {
        this.transform.SetParent(SkillSpawn);
        curSoundPlayer = Instantiate(SoundPlayer, PlayerController.transform.position + SoundPlayer.transform.position, Quaternion.identity);
        curSoundPlayer.transform.SetParent(SkillSpawn);
        curSoundPlayer.Play();
    }

    void PlayShotSound()
    {
        int clipNumber = Random.Range(0, SoundClips.Length);
        GunSound.clip = SoundClips[clipNumber];
        GunSound.Play();
    }

    public override void Shoot()
    {
        if (Time.time > nextSoundTime)
        {
            GunSound.Play();
            nextSoundTime = Time.time + SBetweenSounds + curSoundPlayer.GetClipLenght();
            curSoundPlayer.Play();
        }

        if (Time.time > nextShotTime && base.PlayerController.curActionPoints > 0 && !base.PlayerController.isInAction)
        {
            nextShotTime = Time.time + MsBetweenShot * 0.001f;

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
            //playShotSound();

            MuzzleFlash.Activate();

            //Debug.Log("Fire");

            base.PlayerController.curActionPoints -= ActionPointsCost;
        }
        else if (Time.time > nextShotTime && base.PlayerController.curActionPoints <= 0)
        {
            GunSound.PlayOneShot(SoundEmpty);
            nextShotTime = Time.time + MsBetweenShot / 150;
        }
    }
}
