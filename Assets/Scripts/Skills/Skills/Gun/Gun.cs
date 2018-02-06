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

    MuzzleFlash MuzzleFlash;
    AudioSource GunSound;
    public AudioClip[] soundClips;

    public Transform shell;
    public Transform shellEjection;

    float nextShotTime;

    void playSound()
    {
        int clipNumber = Random.Range(0, soundClips.Length);
        GunSound.clip = soundClips[clipNumber];
        GunSound.Play();
    }

    public override void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + MsBetweenShot / 1000;

            Quaternion accuracy = Quaternion.Euler(Random.Range(-AccuracyHorizontal, AccuracyHorizontal), Random.Range(-AccuracyVertical, AccuracyVertical), 0);

            Projectile newProjectile = Instantiate(Projectile, Muzzle.position, Muzzle.rotation * accuracy) as Projectile;
            newProjectile.SetSpeed(MuzzleVelocity);

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            playSound();
            MuzzleFlash.Activate();

            Debug.Log("Fire");
        }
    }
}
