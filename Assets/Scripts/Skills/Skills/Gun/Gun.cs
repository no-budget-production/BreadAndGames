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

    public MuzzleFlash MuzzleFlash;
    public AudioSource GunSound;
    public AudioClip[] SoundClips;
    public AudioClip SoundEmpty;

    public Transform Shell;
    public Transform ShellEjection;

    private float nextShotTime;

    private void Start()
    {
        this.transform.SetParent(SkillSpawn);
    }

    void playShotSound()
    {
        int clipNumber = Random.Range(0, SoundClips.Length);
        GunSound.clip = SoundClips[clipNumber];
        GunSound.Play();
    }

    public override void Shoot()
    {
        if (Time.time > nextShotTime && base.PlayerController.curActionPoints > 0 && !base.PlayerController.isInAction)
        {
            nextShotTime = Time.time + MsBetweenShot * 0.001f;

            Quaternion accuracy = Quaternion.Euler(Random.Range(-AccuracyHorizontal, AccuracyHorizontal), Random.Range(-AccuracyVertical, AccuracyVertical), 0);

            Projectile newProjectile = Instantiate(Projectile, Muzzle.position, Muzzle.rotation * accuracy) as Projectile;
            newProjectile.SetSpeed(MuzzleVelocity);

            Instantiate(Shell, ShellEjection.position, ShellEjection.rotation);
            playShotSound();
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
