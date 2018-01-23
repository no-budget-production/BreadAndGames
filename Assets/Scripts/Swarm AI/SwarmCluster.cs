using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmCluster : MonoBehaviour
{

    public float SphereRadius;
    public bool DrawCheckSphere;
    public Transform[] Waypoints;
    public float ObjectBornTime;

    //test variables
    public Transform Destination;
    
    private int NumberOfEnemys;
    public int _NumberOfEnemys { get { return NumberOfEnemys; } }
    private List<GameObject> AllEnemysInCluster;
    private SwarmController[] SwarmControllerScripts;
    private NavMeshAgent NavMeshAgent;


    void Awake()
    {
        AllEnemysInCluster = new List<GameObject>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        GetAllEnemysInCluster();
        ObjectBornTime = Time.realtimeSinceStartup;

        //SphereCollider hitbox = gameObject.AddComponent<SphereCollider>() as SphereCollider;
        //hitbox.radius = SphereRadius;
        //hitbox.isTrigger = true;
    }

    void Update()
    {
        All_GoToWaypoints();
        if (Destination != null)
        {
            ClusterMoveToDestination(Destination);
        }        
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

    
    public void GetAllEnemysInCluster()
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

    public void ClusterMoveToDestination(Transform Destination)
    {
        Vector3 targetVector = Destination.transform.position;
        NavMeshAgent.SetDestination(targetVector);
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SwarmCluster"))
        {
            SwarmCluster TempSwarmClusterScript = other.GetComponent<SwarmCluster>();
            if (NumberOfEnemys > TempSwarmClusterScript._NumberOfEnemys)
            {
                Debug.Log("i can live :)");
                return;
            }
            else if (NumberOfEnemys < TempSwarmClusterScript._NumberOfEnemys)
            {
                Debug.Log("i have to die :(");
                for (int i = 0; i < AllEnemysInCluster.Count; i++)
                {
                    AllEnemysInCluster[i].transform.parent = other.transform;
                }
                Destroy(gameObject);
            }
            else if (NumberOfEnemys == TempSwarmClusterScript._NumberOfEnemys)
            {
                Debug.Log("dice");
            }

        }
    }
}
