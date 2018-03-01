using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpawner : MonoBehaviour
{
    public GameObject PrefabToSpawn;            // The prefab what should be spawned

    public Transform HoldingPoint;

    public PhaseOneLevelScript PhaseOneLevelScript;

    private Transform EnemyHolder;

    private float Timer;

    private bool StartSpawning;
    public bool _StartSpawning { get { return StartSpawning; } set { StartSpawning = value; } }

    private float Interval;
    public float _Interval { get { return Interval; } set { Interval = value; } }

    private float AmountToSpawn;
    public float _AmountOfEnemys { get { return AmountToSpawn; } set { AmountToSpawn = value; } }

    private int SpawnedEnemys;
    public int _SpawnedEnemys { get { return SpawnedEnemys; } set { SpawnedEnemys = value; } }

    void Awake()
    {
        Timer = 0;
        Interval = 0;
        AmountToSpawn = 0;
        StartSpawning = false;
    }

    private void Start()
    {

        EnemyHolder = GameManager.Instance.EnemyHolder;
    }

    void Update()
    {
        if (StartSpawning)
        {
            if (SpawnedEnemys < AmountToSpawn)
            {
                if (Timer < Interval)
                {
                    Timer += Time.deltaTime;

                }
                else
                {
                    SpawnEnemy();
                    Timer = 0;
                }

            }

            if (SpawnedEnemys >= AmountToSpawn)
            {
                DisableSpawn();
            }

        }
    }

    public void SpawnEnemy()
    {
        GameObject curPrefab;
        curPrefab = Instantiate(PrefabToSpawn, transform.position, transform.rotation) as GameObject;
        SpawnedEnemys++;
        PhaseOneLevelScript._AmountOfSpawnedEnemysInCurrentWave++;
        curPrefab.transform.parent = EnemyHolder.transform;
    }

    public void DisableSpawn()
    {
        this.enabled = false;
        StartSpawning = false;
        SpawnedEnemys = 0;
    }

}

