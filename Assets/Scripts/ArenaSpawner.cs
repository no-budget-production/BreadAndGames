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

    void Start()
    {
        WaitAndSpawnCoroutine = WaitAndSpawn(SpawnRate);
        StartCoroutine(WaitAndSpawnCoroutine);
        EnemyHolder = GameManager.Instance.EnemyHolder;
    }

    void SpawnEnemy()
    {
        // Spawn one enemy
        GameObject curPrefab;
        curPrefab = Instantiate(PrefabToSpawn, transform.position, transform.rotation) as GameObject;
        curPrefab.transform.parent = SwarmHandler.transform;


        // Let the spawned enemy move to the Holding Point
        var curEnemy = curPrefab.GetComponent<Enemy>();
        if (curEnemy != null)
        {
            GameManager.Instance.Enemies.Add(curEnemy);
            //    curEnemy.NavMeshAgent.SetDestination(HoldingPoint.transform.position);
            //    curEnemy.ReinforcmentPoints = ReinforcmentPoints;
        }

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

