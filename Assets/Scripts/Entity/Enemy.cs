using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    [Header(">>>>>>>>>> Enemy:")]

    public List<Transform> PlayerInRadius = new List<Transform>();

    public List<Transform> PlayersInRange = new List<Transform>();

    public Transform Target;

    public Skill[] UsedSkills;

    public bool isGameOver;
    public bool isAlive;
    public bool isInRange;
    public bool isTargetInView;
    public bool isSooting;

    public GameObject deadExplosionPrefab;
    public GameObject gameOverEffect;

    public float MaxShootingRange;
    public float StopRange;
    public float LookDamping;

    private Vector3 direction;

    public int EveryXFramesAliveCheck;
    public int FrameCounterAliveCheck;
    public int EveryXFramesDistanceCheck;
    public int FrameCounterDistanceCheck;
    public int EveryXFramesRayCheck;
    public int FrameCounterRayCheck;
    public int EveryXFramesShoot;
    public int FrameCounterShoot;
    public int EveryXFramesFind;
    public int FrameCounterFind;

    [HideInInspector]
    public float TempSpeed;
    public float NavAgentSpeed;

    private int animMovX = Animator.StringToHash("MovX");

    private NavMeshAgent NavMeshAgent;

    public float ChanceToSpawnHPPickUps;

    public bool Stunned;
    public float SBetweenUnStun;
    private float nextStunTime;

    public override void Start()
    {
        base.Start();

        GameManager.Instance.Enemies.Add(this);

        NavMeshAgent = GetComponent<NavMeshAgent>();

        NavAgentSpeed = NavMeshAgent.speed;

        SkillSetup();
    }

    public override void OnCustomDestroy()
    {
        if (deadExplosionPrefab != null)
        {
            GameObject TempObjectHolder = Instantiate(deadExplosionPrefab, new Vector3(thisTransform.position.x, 1f, thisTransform.position.z), thisTransform.rotation) as GameObject;
        }

        GameManager.Instance.Enemies.Remove(this);

        if (DiedAmount == 1)
        {
            if (GameManager.Instance.PickUpSpawner.HealthPickUpsSpawn)
            {
                float randomNumber = (Random.Range(0, 100.0f));
                if (randomNumber <= ChanceToSpawnHPPickUps)
                {
                    GameManager.Instance.PickUpSpawner.SpawnPickUps(thisTransform.position);
                }
            }
        }

        base.OnCustomDestroy();
    }

    public void SkillSetup()
    {
        ActiveSkills = new Skill[UsedSkills.Length];

        for (int i = 0; i < UsedSkills.Length; i++)
        {
            Skill curSkill = Instantiate(UsedSkills[i], thisTransform.position + UsedSkills[i].transform.position, Quaternion.identity, thisTransform);
            //curSkill.transform.SetParent(transform);
            curSkill.Character = this;
            curSkill.SkillSpawn = SkillSpawn;
            curSkill.transform.position = SkillSpawn.position;
            curSkill.transform.rotation = SkillSpawn.rotation;
            ActiveSkills[i] = curSkill;
        }

        for (int i = 0; i < ActiveSkills.Length; i++)
        {
            ActiveSkills[i].LateSkillSetup();
        }

        if (GetNearestTargetWithNavMesh())
        {
            NavMeshAgent.destination = Target.position;
        }
        //else
        //{
        //    //NavMeshAgent.enabled = false;
        //}
    }

    public override void Update()
    {
        if (!Stunned)
        {
            if (!isGameOver)
            {

                base.Update();
            }
        }
    }


    public override void FixedUpdate()
    {
        _Animtor.SetFloat(animMovX, TempSpeed);
        if (!Stunned)
        {
            //TurnSpeed = NavMeshAgent.transform.InverseTransformDirection(NavMeshAgent.velocity).normalized;
            TempSpeed = NavMeshAgent.velocity.magnitude / NavAgentSpeed;
            //Anim.SetFloat(animMovX, TempSpeed + TurnSpeed.magnitude / NavMeshAgent.angularSpeed);

            if (!isGameOver)
            {

                base.FixedUpdate();

                FrameCounterAliveCheck++;
                if ((FrameCounterAliveCheck % EveryXFramesAliveCheck) == 0)
                {
                    isAlive = CheckIsAlive(Target);
                    FrameCounterAliveCheck = 0;
                }

                if (isAlive)
                {
                    LockOn();

                    FrameCounterDistanceCheck++;
                    if ((FrameCounterDistanceCheck % EveryXFramesDistanceCheck) == 0)
                    {
                        isInRange = InRange(Target, MaxShootingRange);
                        FrameCounterDistanceCheck = 0;
                    }

                    if (isInRange)
                    {
                        FrameCounterRayCheck++;
                        if ((FrameCounterRayCheck % EveryXFramesRayCheck) == 0)
                        {
                            //LockOn();
                            isTargetInView = CheckView();
                            FrameCounterRayCheck = 0;
                        }

                        if (isTargetInView)
                        {
                            FrameCounterShoot++;
                            if ((FrameCounterShoot % EveryXFramesShoot) == 0)
                            {
                                for (int i = 0; i < ActiveSkills.Length; i++)
                                {
                                    ActiveSkills[i].Shoot();
                                }
                                FrameCounterShoot = 0;

                                isSooting = true;
                            }
                        }
                        else
                        {
                            FrameCounterFind++;
                            if ((FrameCounterFind % EveryXFramesFind) == 0)
                            {
                                GetNearestTargetWithNavMesh();
                                FrameCounterFind = 0;
                            }

                            isSooting = false;
                        }
                    }
                    else
                    {
                        isTargetInView = false;
                        isSooting = false;
                    }

                    if (!isSooting)
                    {
                        FrameCounterFind++;
                        if ((FrameCounterFind % EveryXFramesFind) == 0)
                        {
                            if (GetNearestTargetWithNavMesh())
                            {
                                NavMeshAgent.destination = Target.position;
                            }
                            else
                            {
                                isGameOver = true;
                            }
                            FrameCounterFind = 0;
                        }


                    }
                    else
                    {
                        if (GetNearestTargetWithNavMesh())
                        {
                            NavMeshAgent.destination = Target.position;
                        }
                        else
                        {
                            isGameOver = true;
                        }
                    }

                }
                else
                {
                    if (GetNearestTargetWithNavMesh())
                    {
                        NavMeshAgent.destination = Target.position;
                    }
                    else
                    {
                        isGameOver = true;
                    }

                }
            }
        }
        else
        {
            NavMeshAgent.destination = thisTransform.position;

            if (Time.time > nextStunTime)
            {
                Stunned = false;
                gameOverEffect.SetActive(false);
            }
        }

    }

    public void Ebomb()
    {
        gameOverEffect.SetActive(true);
        Stunned = true;
        nextStunTime = Time.time + SBetweenUnStun;
    }

    bool GetNearestTargetWithNavMesh()
    {
        PlayerInRadius.Clear();

        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            Transform tempPlayer = GameManager.Instance.Players[i].GetTransform();

            if (CheckIsAlive(tempPlayer))
            {
                PlayerInRadius.Add(tempPlayer);
            }
        }

        int NearestPlayer = 0;
        float ShortestWay = 0;
        NavMeshPath Path = new NavMeshPath();
        //bool PlayersInRange = false;

        if (PlayerInRadius.Count > 0)
        {
            for (int i = 0; i < PlayerInRadius.Count; i++)
            {
                float LenghtSoFar = 0f;

                if (PlayerInRadius[i] != null)
                {
                    //PlayersInRange = true;
                    NavMeshAgent.CalculatePath(PlayerInRadius[i].position, Path);

                    for (int i2 = 0; i2 < Path.corners.Length; i2++)
                    {
                        Vector3 previousCorner = Path.corners[0];
                        Vector3 currentCorner = Path.corners[i2];

                        LenghtSoFar += Vector3.Distance(previousCorner, currentCorner);

                        previousCorner = currentCorner;
                    }

                    if (ShortestWay == 0)
                    {
                        ShortestWay = LenghtSoFar;
                    }
                    else if (LenghtSoFar < ShortestWay)
                    {
                        ShortestWay = LenghtSoFar;
                        NearestPlayer = i;
                    }
                }
            }

            Target = PlayerInRadius[NearestPlayer];

            return true;
        }
        else
        {
            return false;
        }
    }

    bool InRange(Transform transformTarget, float MaxRange)
    {
        if (Vector3.Distance(transformTarget.position, thisTransform.position) > MaxRange)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void LockOn()
    {
        //transform.LookAt(Target.position);

        var lookPos = new Vector3(Target.position.x, 0, Target.position.z) - new Vector3(thisTransform.position.x, 0, thisTransform.position.z);
        lookPos.y = 0;
        if (lookPos != Vector3.zero)
        {
            var rotation = Quaternion.LookRotation(lookPos);
            thisTransform.rotation = Quaternion.Slerp(thisTransform.rotation, rotation, Time.deltaTime * LookDamping);
        }
        //direction = Target.position - transform.position;
        //Quaternion lookRotation = Quaternion.LookRotation(direction);
        //Vector3 rotation = Quaternion.Lerp(Target.rotation, lookRotation, Time.deltaTime * 100000000).eulerAngles;
        //Target.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    bool CheckView()
    {
        NavMeshHit hit;

        if (!NavMeshAgent.Raycast(Target.position, out hit))
        {
            return true;
        }
        return false;
    }

    bool CheckIsAlive(Transform transformArg)
    {
        if (transformArg != null)
        {
            var tempEntity = transformArg.GetComponent<Entity>();
            if (tempEntity != null)
            {
                if (tempEntity.CurrentHealth > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void FindAlivePlayer()
    {
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            Transform tempPlayer = GameManager.Instance.Players[i].GetTransform();

            if (CheckIsAlive(tempPlayer))
            {
                PlayerInRadius.Add(tempPlayer);
            }
        }
    }
}