using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : MonoBehaviour {

    public GameObject BulletSpawn;
    public GameObject BulletPrefab;

    public float DamageBonus;

    public float msBetweenShot = 100;
    public float muzzleVelocity = 35;

    private float nextShotTime;

    public void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShot / 1000;

            Quaternion accuracy = Quaternion.Euler(Random.Range(-1.0f, 1.0f), Random.Range(-3.0f, 3.0f), 0);

            //The Bullet instantiation happens here.
            GameObject TemporaryBulletHandler;
            TemporaryBulletHandler = Instantiate(BulletPrefab, BulletSpawn.transform.position, BulletSpawn.transform.rotation * accuracy) as GameObject;
            var RayCollider = TemporaryBulletHandler.GetComponent<RayCollider>();
            RayCollider.OriginGameObject = gameObject;
            RayCollider.BonusDamage = DamageBonus;
        }
    }
}
