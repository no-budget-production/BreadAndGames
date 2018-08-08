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

    private Transform thisTransform;

    void Awake()
    {
        Timer = 0;
        Interval = 0;
        AmountToSpawn = 0;
        StartSpawning = false;
        thisTransform = GetComponent<Transform>();
    }

#if UNITY_EDITOR
    private void Start()
    {
        EnemyHolder = GameManager.Instance.EnemyHolder;
    }
#endif

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
        GameObject curPrefab = Instantiate(PrefabToSpawn, thisTransform.position, thisTransform.rotation);
        SpawnedEnemys++;
        PhaseOneLevelScript._AmountOfSpawnedEnemysInCurrentWave++;
#if UNITY_EDITOR
        curPrefab.transform.parent = EnemyHolder.transform;
#endif
    }

    public void DisableSpawn()
    {
        this.enabled = false;
        StartSpawning = false;
        SpawnedEnemys = 0;
    }

}

