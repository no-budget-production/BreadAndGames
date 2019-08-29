﻿//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public class SwarmCluster : MonoBehaviour
//{
//    public float CheckRadius;
//    public Transform[] Waypoints;
//    public float ChaseDistance;
//    public Transform[] ReinforcmentPoints;
//    public GameObject PrefabToSpawn;

//    private int OldEnemyCount;
//    private int PlayerCount;
//    private GameObject Target;

//    private float RandomNumber;
//    public float _RandomNumber { get { return RandomNumber; } }

//    private List<GameObject> AllEnemysInCluster;
//    public List<GameObject> _AllEnemysInCluster { get { return AllEnemysInCluster; } set { AllEnemysInCluster = value; } }

//    private NavMeshAgent NavMeshAgent;
//    public NavMeshAgent _NavMeshAgent { get { return NavMeshAgent; } }

//    private GameObject[] PlayerInRadius;
//    private List<GameObject> PlayerInRadiusList;        // needed?

//    private List<SwarmController> SwarmControllerScripts;
//    public List<SwarmController> _SwarmControllerScripts { get { return SwarmControllerScripts; } set { SwarmControllerScripts = value; } }


//    void Awake()
//    {
//        SwarmControllerScripts = new List<SwarmController>();
//        AllEnemysInCluster = new List<GameObject>();
//        PlayerInRadiusList = new List<GameObject>();
//        NavMeshAgent = GetComponent<NavMeshAgent>();
//        PlayerInRadius = new GameObject[3];

//        // Spawn one enemy
//        GameObject TempSpawnHandler;
//        TempSpawnHandler = Instantiate(PrefabToSpawn, transform.position, transform.rotation) as GameObject;
//        TempSpawnHandler.transform.parent = this.transform;

//        //GameManager.Instance.SwarmController.Add(TempSpawnHandler);

//        GetAllEnemysInCluster();

//        SphereCollider hitbox = gameObject.AddComponent<SphereCollider>() as SphereCollider;
//        hitbox.radius = CheckRadius;
//        hitbox.isTrigger = true;
//    }

//    void Start()
//    {
//        PlayerCount = 0;
//        GetAllEnemysInCluster();
//        RandomNumber = Random.Range(0f, 100f);
//        NavMeshAgent.speed = 10f;


//    }

//    void Update()
//    {
//        if (Target == null)
//        {
//            AllGoToWaypoints();
//        }
//        else
//        {
//            Attack();
//        }

//        if (OldEnemyCount < AllEnemysInCluster.Count)
//        {
//            GetAllEnemysInCluster();
//        }

//        // Death of the cluster
//        if (AllEnemysInCluster.Count <= 0)
//        {
//            Destroy(gameObject);
//        }

//        //if (PlayerCount == 1 & EnemyCount >= 3)
//        //{
//        //    if (Target != null)
//        //    {
//        //        NavMeshAgent.SetDestination(Target.transform.position);
//        //        if (NavMeshAgent.remainingDistance > ChaseDistance)
//        //        {
//        //            Target = null;
//        //            NavMeshAgent.ResetPath();
//        //            GetNearestTarget();
//        //        }
//        //    }
//        //}

//    }

//    void Attack()
//    {
//        NavMeshAgent.SetDestination(Target.transform.position);
//        for (int i = 0; i < SwarmControllerScripts.Count; i++)
//        {
//            if (SwarmControllerScripts[i] != null)
//            {
//                SwarmControllerScripts[i].AttackCommand(Target.transform);
//            }
//        }
//    }

//    public void GetAllEnemysInCluster()
//    {
//        SwarmControllerScripts.Clear();
//        AllEnemysInCluster.Clear();
//        int children = transform.childCount;        // How many childrens are in this cluster?
//        OldEnemyCount = children - 1;               // Safe the number subtract with 1. (the first child are the waypoints)
//        for (int i = 1; i < children; ++i)          // int i = 1 because child(0) are the waypoints
//        {
//            GameObject TempGameObjectHandler = gameObject.transform.GetChild(i).gameObject;
//            SwarmController TempScriptHandler = TempGameObjectHandler.GetComponent<SwarmController>();

//            TempScriptHandler._SwarmClusterScript = this;
//            TempScriptHandler._IndexNumber = i;

//            SwarmControllerScripts.Add(TempScriptHandler);
//            AllEnemysInCluster.Add(TempGameObjectHandler);
//        }
//    }

//    void AllGoToWaypoints()
//    {
//        for (int i = 0; i < SwarmControllerScripts.Count; i++)
//        {
//            if (SwarmControllerScripts[i] != null)
//            {
//                int tempI;
//                if (i > 14)
//                {
//                    tempI = 14;
//                    SwarmControllerScripts[i].MoveToDestination(Waypoints[tempI]);
//                }
//                else
//                {
//                    SwarmControllerScripts[i].MoveToDestination(Waypoints[i]);
//                }

//            }
//        }
//    }

//    void ClusterTakeover(Collider OpponentCluster, SwarmCluster OpponentClusterScript)
//    {
//        for (int i = 0; i < AllEnemysInCluster.Count; i++)
//        {
//            if (AllEnemysInCluster[i] == null)
//            {
//                AllEnemysInCluster.Remove(AllEnemysInCluster[i]);
//                i--;
//            }
//            AllEnemysInCluster[i].transform.parent = OpponentCluster.transform;
//        }
//        OpponentClusterScript.GetAllEnemysInCluster();
//        Destroy(gameObject);
//    }

//    void GetNearestTarget()
//    {
//        float LenghtSoFar = 0f;
//        int NearestPlayer = 0;
//        float ShortestWay = 100;
//        NavMeshPath Path = new NavMeshPath();
//        bool PlayersInRange = false;
//        for (int i = 0; i < PlayerInRadius.Length; i++)
//        {
//            if (PlayerInRadius[i] != null)
//            {
//                PlayersInRange = true;
//                NavMeshAgent.CalculatePath(PlayerInRadius[i].transform.position, Path);

//                for (int i2 = 0; i2 < Path.corners.Length; i2++)
//                {
//                    Vector3 previousCorner = Path.corners[0];
//                    Vector3 currentCorner = Path.corners[i2];

//                    LenghtSoFar += Vector3.Distance(previousCorner, currentCorner);

//                    previousCorner = currentCorner;
//                }

//                if (LenghtSoFar < ShortestWay)
//                {
//                    ShortestWay = LenghtSoFar;
//                    NearestPlayer = i;
//                }
//            }
//        }

//        if (PlayersInRange == true)
//        {
//            Target = PlayerInRadius[NearestPlayer];
//        }
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("SwarmCluster"))
//        {
//            SwarmCluster TempSwarmClusterScript = other.GetComponent<SwarmCluster>();
//            if (AllEnemysInCluster.Count > TempSwarmClusterScript._AllEnemysInCluster.Count)
//            {
//                // this.Cluster is bigger. No action needed.
//                return;
//            }
//            else if (AllEnemysInCluster.Count < TempSwarmClusterScript._AllEnemysInCluster.Count)
//            {
//                // this.Cluster is smaller. Hand over all enemys and destroy yourself.
//                ClusterTakeover(other, TempSwarmClusterScript);
//            }
//            else if (AllEnemysInCluster.Count == TempSwarmClusterScript._AllEnemysInCluster.Count)
//            {
//                // Clusters have the same amount of enemys. Do the takeover with random numbers.
//                if (RandomNumber > TempSwarmClusterScript._RandomNumber)
//                {
//                    return;
//                }
//                else if (RandomNumber < TempSwarmClusterScript._RandomNumber)
//                {
//                    ClusterTakeover(other, TempSwarmClusterScript);
//                }
//            }
//            else
//            {
//                Debug.Log("Cluster Takeover Error");
//            }
//        }

//        if (other.CompareTag("Melee"))
//        {
//            PlayerCount++;
//            PlayerInRadius[0] = other.gameObject;
//            PlayerInRadiusList.Add(other.gameObject);
//            if (Target == null)
//            {
//                Target = other.gameObject;
//            }
//        }
//        if (other.CompareTag("Shooter"))
//        {
//            PlayerCount++;
//            PlayerInRadius[1] = other.gameObject;
//            PlayerInRadiusList.Add(other.gameObject);
//            if (Target == null)
//            {
//                Target = other.gameObject;
//            }
//        }
//        if (other.CompareTag("Support"))
//        {
//            PlayerCount++;
//            PlayerInRadius[2] = other.gameObject;
//            PlayerInRadiusList.Add(other.gameObject);
//            if (Target == null)
//            {
//                Target = other.gameObject;
//            }
//        }
//    }

//    void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Melee"))
//        {
//            PlayerCount--;
//            PlayerInRadius[0] = null;
//            PlayerInRadiusList.Remove(other.gameObject);

//        }
//        if (other.CompareTag("Shooter"))
//        {
//            PlayerCount--;
//            PlayerInRadius[1] = null;
//            PlayerInRadiusList.Remove(other.gameObject);

//        }
//        if (other.CompareTag("Support"))
//        {
//            PlayerCount--;
//            PlayerInRadius[2] = null;
//            PlayerInRadiusList.Remove(other.gameObject);
//        }
//    }
//}