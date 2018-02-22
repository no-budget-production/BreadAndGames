using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpawner : MonoBehaviour
{
    public GameObject PrefabToSpawn;            // The prefab what should be spawned
    
    public float SpawnRate;                     // The time limit when the next enemy will spawn
    public int AmountToSpawn;                   // How many enemys should spawn

    public Transform HoldingPoint;

    private Transform EnemyHolder;
    

    private IEnumerator WaitAndSpawnCoroutine;
    private int SpawnedEnemys;
    public int _SpawnedEnemys { get { return SpawnedEnemys; } set { SpawnedEnemys = value; } }



    void Start()
    {
        WaitAndSpawnCoroutine = WaitAndSpawn(SpawnRate);
        StartCoroutine(WaitAndSpawnCoroutine);
        EnemyHolder = GameManager.Instance.EnemyHolder;
    }

    void SpawnEnemy()
    {
        GameObject curPrefab;
        curPrefab = Instantiate(PrefabToSpawn, transform.position, transform.rotation) as GameObject;
        curPrefab.transform.parent = EnemyHolder.transform;
        

        SpawnedEnemys++;

        if (SpawnedEnemys == AmountToSpawn)
        {
            StopCoroutine(WaitAndSpawnCoroutine);
        }
    }

    private IEnumerator WaitAndSpawn(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            SpawnEnemy();
        }
    }
}

