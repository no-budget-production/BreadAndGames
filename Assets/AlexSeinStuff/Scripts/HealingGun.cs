using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealingGun : MonoBehaviour
{

    public Transform muzzle;
    public Projectile projectile;
    public float msBetweenShot = 100;
    public float muzzleVelocity = 35;
    public float reloadTime = 1.5f;
    public int clipSize = 30;

    bool isReloading = false;

    MuzzleFlash muzzleFlash;
    AudioSource gunSound;
    public AudioClip[] soundShootClips;
    public AudioClip soundEmpty;
    public AudioClip soundReload;

    public Transform shell;
    public Transform shellEjection;

    public GameObject circleBar;

    float nextShotTime;
    int defaultMagSize;

    private void Start()
    {
        gunSound = GetComponent<AudioSource>();
        muzzleFlash = GetComponent<MuzzleFlash>();

        defaultMagSize = clipSize;
    }
    void Update()
    {
        if (Input.GetButtonDown("Reload1") && clipSize < defaultMagSize - 1)
        {
            Reload();
        }
    }

    void playShotSound()
    {
        int clipNumber = Random.Range(0, soundShootClips.Length);
        gunSound.clip = soundShootClips[clipNumber];
        gunSound.Play();
    }

    public void Shoot()
    {
        if (Time.time > nextShotTime && clipSize >= 0 && !isReloading)
        {
            nextShotTime = Time.time + msBetweenShot / 1000;

            Quaternion accuracy = Quaternion.Euler(Random.Range(-1.0f, 1.0f), Random.Range(-3.0f, 3.0f), 0);

            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation * accuracy) as Projectile;
            newProjectile.SetSpeed(muzzleVelocity);

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            playShotSound();
            muzzleFlash.Activate();
            clipSize -= 1;

        }
        else if (Time.time > nextShotTime && clipSize <= 0)
        {
            gunSound.PlayOneShot(soundEmpty);
            nextShotTime = Time.time + msBetweenShot / 150;
        }
    }

    public void Reload()
    {
        isReloading = true;
        gunSound.PlayOneShot(soundReload);
        clipSize = 30;
        nextShotTime = Time.time + reloadTime;

        isReloading = false;
    }


} 







