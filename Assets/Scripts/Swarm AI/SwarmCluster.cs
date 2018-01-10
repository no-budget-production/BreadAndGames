using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmCluster : MonoBehaviour
{

    public float SphereRadius;
    public bool DrawCheckSphere;
    public Transform[] Waypoints;

    public Transform bla;
    

    [HideInInspector]
    public int NumberOfEnemys;

    private List<GameObject> AllEnemysInCluster;
    private SwarmController[] SwarmControllerScripts;
    private Transform dest;
    private NavMeshAgent NavMeshAgent;

    void Awake()
    {
        AllEnemysInCluster = new List<GameObject>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        GetAllEnemysInCluster();
    }

    void Update()
    {
        All_GoToWaypoints();
        MoveToDestination(bla);
    }

    // Draw a gizmo sphere for visibilty of the checkSphere and debugging
    void OnDrawGizmosSelected()
    {
        if (DrawCheckSphere)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, SphereRadius);
        }

    }

    void GetAllEnemysInCluster()
    {
        int children = transform.childCount;
        NumberOfEnemys = children - 1;
        SwarmControllerScripts = new SwarmController[NumberOfEnemys];
        for (int i = 1; i < children; ++i)  // int i = 1, because you have to skip the first child. The First one are the waypoints.
        {
            GameObject TempGameObjectHandler = gameObject.transform.GetChild(i).gameObject;
            SwarmController TempScriptHandler = TempGameObjectHandler.GetComponent<SwarmController>();

            AllEnemysInCluster.Add(TempGameObjectHandler);
            SwarmControllerScripts[i - 1] = TempScriptHandler;
        }

    }

    void All_GoToWaypoints()
    {
        for (int i = 0; i < SwarmControllerScripts.Length; ++i)
        {
            SwarmControllerScripts[i].MoveToDestination(Waypoints[i]);
        }
    }

    void MoveToDestination(Transform Destination)
    {
        Vector3 targetVector = Destination.transform.position;
        NavMeshAgent.SetDestination(targetVector);
    }
}
