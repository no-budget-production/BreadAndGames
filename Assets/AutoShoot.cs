using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShoot : MonoBehaviour {

    public EnemyTurret Turret;
    public float ShootDelay;
    public float ShootInterval;

    void Start()
    {
        InvokeRepeating("RepeatingShooting", ShootDelay, ShootInterval);
    }

    private void RepeatingShooting()
    {
        Turret.Shoot();
    }
}
