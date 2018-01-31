using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmCluster : MonoBehaviour
{

    public float CheckRadius;
    public Transform[] Waypoints;
    public float ChaseDistance;

    private Transform Reinforcement;
    
    private int NumberOfEnemys;
    public int _NumberOfEnemys { get { return NumberOfEnemys; } }
    private float RandomNumber;
    public float _RandomNumber { get { return RandomNumber; } }
    private List<GameObject> AllEnemysInCluster;
    private SwarmController[] SwarmControllerScripts;   // Using a array here, because a list doesn't work (don't know why)
    private NavMeshAgent NavMeshAgent;
    public NavMeshAgent _NavMeshAgent { get { return NavMeshAgent; } }
    private GameObject[] PlayerInRadius;
    private List<GameObject> PlayerInRadiusList;
    private GameObject Target;

    void Awake()
    {
        AllEnemysInCluster = new List<GameObject>();
        PlayerInRadiusList = new List<GameObject>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        SwarmControllerScripts = new SwarmController[15];
        PlayerInRadius = new GameObject[3];
        Reinforcement = GameObject.Find("Reinforcment").GetComponent<Transform>();

        SphereCollider hitbox = gameObject.AddComponent<SphereCollider>() as SphereCollider;
        hitbox.radius = CheckRadius;
        hitbox.isTrigger = true;
    }

    void Start()
    {
        GetAllEnemysInCluster();
        RandomNumber = Random.Range(0f, 100f);
        NavMeshAgent.speed = 10f;
    }

    void Update()
    {
        All_GoToWaypoints();

        if (PlayerInRadiusList.Count == 1 & AllEnemysInCluster.Count >= 3)
        {
            if (Target != null)
            {
                NavMeshAgent.SetDestination(Target.transform.position);
                if (NavMeshAgent.remainingDistance > ChaseDistance)
                {
                    Target = null;
                    NavMeshAgent.ResetPath();
                    GetNearestTarget();
                }
            }
        }
        if (PlayerInRadiusList.Count == 1 & AllEnemysInCluster.Count <= 2)
        {
            NavMeshAgent.SetDestination(Reinforcement.position);
            Debug.Log(NavMeshAgent.speed);
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

    void ClusterTakeover(Collider OpponentCluster, SwarmCluster OpponentClusterScript)
    {
        for (int i = 0; i < AllEnemysInCluster.Count; i++)
        {
            AllEnemysInCluster[i].transform.parent = OpponentCluster.transform;
        }
        OpponentClusterScript.GetAllEnemysInCluster();
        Destroy(gameObject);
    }
    
    void GetNearestTarget()
    {
        float LenghtSoFar = 0f;
        int NearestPlayer = 0;
        float ShortestWay = 100;
        NavMeshPath Path = new NavMeshPath();
        bool PlayersInRange = false;
        for (int i = 0; i < PlayerInRadius.Length; i++)
        {
            if (PlayerInRadius[i] != null)
            {
                PlayersInRange = true;
                NavMeshAgent.CalculatePath(PlayerInRadius[i].transform.position, Path);
                
                for (int i2 = 0; i2 < Path.corners.Length; i2++)
                {
                    Vector3 previousCorner = Path.corners[0];
                    Vector3 currentCorner = Path.corners[i2];

                    LenghtSoFar += Vector3.Distance(previousCorner, currentCorner);
                    
                    previousCorner = currentCorner;
                }

                if (LenghtSoFar < ShortestWay)
                {
                    ShortestWay = LenghtSoFar;
                    NearestPlayer = i;
                }
            }


        }

        if (PlayersInRange == true)
        {
            Target = PlayerInRadius[NearestPlayer];
        }
        
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
            PlayerInRadiusList.Add(other.gameObject);
            if (Target == null)
            {
                Target = other.gameObject;
            }
        }
        if (other.CompareTag("Shooter"))
        {
            PlayerInRadius[1] = other.gameObject;
            PlayerInRadiusList.Add(other.gameObject);
            if (Target == null)
            {
                Target = other.gameObject;
            }
        }
        if (other.CompareTag("Support"))
        {
            PlayerInRadius[2] = other.gameObject;
            PlayerInRadiusList.Add(other.gameObject);
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
            PlayerInRadiusList.Remove(other.gameObject);

        }
        if (other.CompareTag("Shooter"))
        {
            PlayerInRadius[1] = null;
            PlayerInRadiusList.Remove(other.gameObject);

        }
        if (other.CompareTag("Support"))
        {
            PlayerInRadius[2] = null;
            PlayerInRadiusList.Remove(other.gameObject);
        }
    }
}
