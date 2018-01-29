using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmCluster : MonoBehaviour
{

    public float CheckRadius;
    public Transform[] Waypoints;

    //test variables
    public Transform Destination;
    
    private int NumberOfEnemys;
    public int _NumberOfEnemys { get { return NumberOfEnemys; } }
    private float RandomNumber;
    public float _RandomNumber { get { return RandomNumber; } }
    private List<GameObject> AllEnemysInCluster;
    private SwarmController[] SwarmControllerScripts;   // Using a array here, because a list doesn't work (don't know why)
    private NavMeshAgent NavMeshAgent;
    private GameObject[] PlayerInRadius;
    private GameObject Target;

    void Awake()
    {
        AllEnemysInCluster = new List<GameObject>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        SwarmControllerScripts = new SwarmController[15];
        PlayerInRadius = new GameObject[3];

        SphereCollider hitbox = gameObject.AddComponent<SphereCollider>() as SphereCollider;
        hitbox.radius = CheckRadius;
        hitbox.isTrigger = true;
    }

    void Start()
    {
        GetAllEnemysInCluster();
        RandomNumber = Random.Range(0f, 100f);
    }

    void Update()
    {
        All_GoToWaypoints();
        if (Destination != null)
        {
            ClusterMoveToDestination(Destination);
        }        
    }
    
    
    public void GetAllEnemysInCluster()
    {
        AllEnemysInCluster.Clear();
        int children = transform.childCount;                            // How many childrens are in this cluster?
        NumberOfEnemys = children - 1;                                  // Safe the number subtract with 1. (the first child are the waypoints)
        for (int i = 1; i < children; ++i)                              // int i = 1 because child(0) are the waypoints
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
            if (SwarmControllerScripts[i] != null)
            {
                SwarmControllerScripts[i].MoveToDestination(Waypoints[i]);
            }
        }
    }

    public void ClusterMoveToDestination(Transform Destination)
    {
        Vector3 targetVector = Destination.transform.position;
        NavMeshAgent.SetDestination(targetVector);
    }

    void ClusterTakeover(Collider OpponentCluster, SwarmCluster OpponentClusterScript)
    {
        for (int i = 0; i < AllEnemysInCluster.Count; i++)
        {
            AllEnemysInCluster[i].transform.parent = OpponentCluster.transform;
        }
        OpponentClusterScript.GetAllEnemysInCluster();
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SwarmCluster"))
        {
            SwarmCluster TempSwarmClusterScript = other.GetComponent<SwarmCluster>();
            if (NumberOfEnemys > TempSwarmClusterScript._NumberOfEnemys)
            {
                // this.Cluster is bigger. No action needed.
                return;
            }
            else if (NumberOfEnemys < TempSwarmClusterScript._NumberOfEnemys)
            {
                // this.Cluster is smaller. Hand over all enemys and destroy yourself.
                ClusterTakeover(other, TempSwarmClusterScript);
            }
            else if (NumberOfEnemys == TempSwarmClusterScript._NumberOfEnemys)
            {
                // Clusters have the same amount of enemys. Do the takeover with random numbers.
                if (RandomNumber > TempSwarmClusterScript._RandomNumber)
                {
                    return;
                }
                else if (RandomNumber < TempSwarmClusterScript._RandomNumber)
                {
                    ClusterTakeover(other, TempSwarmClusterScript);
                }
            }
            else
            {
                Debug.Log("Cluster Takeover Error");
            }
        }

        if (other.CompareTag("Melee"))
        {
            PlayerInRadius[0] = other.gameObject;
            if (Target == null)
            {
                Target = other.gameObject;
            }
        }
        if (other.CompareTag("Shooter"))
        {
            PlayerInRadius[1] = other.gameObject;
            if (Target == null)
            {
                Target = other.gameObject;
            }
        }
        if (other.CompareTag("Support"))
        {
            PlayerInRadius[2] = other.gameObject;
            if (Target == null)
            {
                Target = other.gameObject;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Melee"))
        {
            PlayerInRadius[0] = null;
        }
        if (other.CompareTag("Shooter"))
        {
            PlayerInRadius[1] = null;
        }
        if (other.CompareTag("Support"))
        {
            PlayerInRadius[2] = null;
        }
    }
}
