//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public class EnemyMelee : Entity
//{


//    private NavMeshAgent NavMeshAgent;

//    private PlayerController Melee;
//    private PlayerController Shooter;
//    private PlayerController Support;

//    private List<Transform> PlayerInRadius;
//    private PlayerController Target;

//    private Animator animator;
//    private Rigidbody thisRigidbody;

//    private float enemySpeed;


//    void Start()
//    {
//        NavMeshAgent = GetComponent<NavMeshAgent>();
//        animator = GetComponentInChildren<Animator>();
//        thisRigidbody = GetComponent<Rigidbody>();

//        Melee = GameManager.Instance.GetMelee();
//        Shooter = GameManager.Instance.GetShooter();
//        Support = GameManager.Instance.GetSupport();

//        PlayerInRadius.Add(Melee.GetComponent<Transform>());
//        PlayerInRadius.Add(Shooter.GetComponent<Transform>());
//        PlayerInRadius.Add(Support.GetComponent<Transform>());

//        //GetNearestTargetWithNavMesh();
//    }

//    void Update()
//    {

//        enemySpeed = thisRigidbody.velocity.magnitude;
//        animator.SetFloat("Speed", Mathf.Clamp(enemySpeed, 0, 1));

//    }

//    void GetNearestTargetWithNavMesh()
//    {
//        float LenghtSoFar = 0f;
//        int NearestPlayer = 0;
//        float ShortestWay = 0;
//        NavMeshPath Path = new NavMeshPath();
//        bool PlayersInRange = false;
//        for (int i = 0; i < PlayerInRadius.Count; i++)
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

//                if (ShortestWay == 0)
//                {
//                    ShortestWay = LenghtSoFar;
//                }
//                else if (LenghtSoFar < ShortestWay)
//                {
//                    ShortestWay = LenghtSoFar;
//                    NearestPlayer = i;
//                }

//            }
//            //Target = PlayerInRadius[NearestPlayer];
//        }

//    }



//}
