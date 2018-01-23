using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmSpawn : MonoBehaviour
{
    public GameObject PrefabToSpawn;        // The prefab what should be spawned
    public float SpawnRate;                 // The time limit when the next enemy will spawn

    private Transform[] SpawnPoints;        // Array of all SpawnPoint (Array[0] is the Hidden Spawn)
    private IEnumerator coroutine;          //
    private int NumberOfSpawns;             // How many spawn points exist
    private int HoldingPointCounter;        // Counts which "Holding Point" is the next one the enemy has to move to

    void Awake()
    {
        HoldingPointCounter = 1;
    }

    void Start()
    {
        // Get all transforms for the spawn points and put them into a array.
        // So the number of spawning enemys is optional.
        // Array[0] has always to be the "Hidden Spawn".
        int children = transform.childCount;
        NumberOfSpawns = children-1;        // -1 is the hidden spawn                               ?überflüssig wenn int i =1 ist?
        SpawnPoints = new Transform[children];
        for (int i = 0; i < children; ++i)
        {
            SpawnPoints[i] = transform.GetChild(i);
        }


        coroutine = WaitAndSpawn(SpawnRate);
        StartCoroutine(coroutine);
        
    }


    void SpawnEnemy()
    {
        // Spawn one enemy
        GameObject TempSpawnHandler;
        TempSpawnHandler = Instantiate(PrefabToSpawn, SpawnPoints[0].position, SpawnPoints[0].rotation) as GameObject;
        TempSpawnHandler.transform.parent = SpawnPoints[HoldingPointCounter].transform;

        // Let the spawned enemy move to the next free "Holding Point"
        SwarmCluster SwarmClusterScript = TempSpawnHandler.GetComponent<SwarmCluster>();
        SwarmClusterScript.ClusterMoveToDestination(SpawnPoints[HoldingPointCounter]);
        HoldingPointCounter++;

        // Reset the HoldingPointCounter after one iteration of all "Holding Points"
        if (HoldingPointCounter > NumberOfSpawns)
        {
            HoldingPointCounter = 1;
            StopCoroutine(coroutine);
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
