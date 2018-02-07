using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmSpawn : MonoBehaviour
{
    public GameObject PrefabToSpawn;            // The prefab what should be spawned
    public float SpawnRate;                     // The time limit when the next enemy will spawn
    public int AmountToSpawn;                   // How many enemys should spawn
    public Transform SpawnPoint;
    public Transform HoldingPoint;
    public Transform ClusterHolder;

    public Transform SwarmHandler;
    public Transform[] ReinforcmentPoints;

    private IEnumerator WaitAndSpawnCoroutine;
    private int SpawnedEnemys;

    void Start()
    {
        WaitAndSpawnCoroutine = WaitAndSpawn(SpawnRate);
        StartCoroutine(WaitAndSpawnCoroutine);
    }

    void SpawnEnemy()
    {
        // Spawn one enemy
        GameObject TempSpawnHandler;
        TempSpawnHandler = Instantiate(PrefabToSpawn, SpawnPoint.position, SpawnPoint.rotation) as GameObject;
        TempSpawnHandler.transform.parent = SwarmHandler.transform;

        // Let the spawned enemy move to the Holding Point
        SwarmCluster SwarmClusterScript = TempSpawnHandler.GetComponent<SwarmCluster>();
        SwarmClusterScript._NavMeshAgent.SetDestination(HoldingPoint.transform.position);
        SwarmClusterScript.ReinforcmentPoints = ReinforcmentPoints;
        SwarmClusterScript.ParentSwarmSpawn = GetComponent<SwarmSpawn>();
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
