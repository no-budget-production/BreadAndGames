using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public Transform muzzle;
    public Projectile projectile;
    public float msBetweenShot = 100;
    public float muzzleVelocity = 35;

    MuzzleFlash muzzleFlash;
    AudioSource gunSound;
    public AudioClip[] soundClips;

    public Transform shell;
    public Transform shellEjection;

    float nextShotTime;

    private void Start()
    {
        gunSound = GetComponent<AudioSource>();
        muzzleFlash = GetComponent<MuzzleFlash>();
    }

    void playSound()
    {
        int clipNumber = Random.Range(0, soundClips.Length);
        gunSound.clip = soundClips[clipNumber];
        gunSound.Play();
    }

    public void Shoot()
    {
        if(Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShot / 1000;

            Quaternion accuracy = Quaternion.Euler(Random.Range(-1.0f, 1.0f), Random.Range(-3.0f, 3.0f), 0);

            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation * accuracy ) as Projectile;
            newProjectile.SetSpeed(muzzleVelocity);

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            playSound();
            muzzleFlash.Activate();

        }
    }


}
