using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : MonoBehaviour {

    public GameObject BulletSpawn;
    public GameObject BulletPrefab;

    public float DamageBonus; 

    void Start()
    {
        InvokeRepeating("Shoot", 2.0f, 0.3f);
    }

    public void Shoot()
    {

        //The Bullet instantiation happens here.
        GameObject TemporaryBulletHandler;
        TemporaryBulletHandler = Instantiate(BulletPrefab, BulletSpawn.transform.position, BulletSpawn.transform.rotation) as GameObject;
        var HitCollider = TemporaryBulletHandler.GetComponent<HitCollider>();
        HitCollider.OriginGameObject = gameObject;
        HitCollider.BonusDamage = DamageBonus;


    }
}
