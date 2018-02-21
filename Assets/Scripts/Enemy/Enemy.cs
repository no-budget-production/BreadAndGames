using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    private NavMeshAgent NavMeshAgent;

    public List<Transform> PlayerInRadius = new List<Transform>();

    public List<Transform> PlayersInRange = new List<Transform>();

    public Transform Target;

    public bool isGameOver;
    public bool isAlive;
    public bool isInRange;
    public bool isTargetInView;
    public bool isSooting;

    public float MaxShootingRange;
    public float StopRange;
    public float LookDamping;

    public Skill[] UsedSkills;

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

    public Animator Anim;

    int animSpeed = Animator.StringToHash("Speed");
    int animIsAttacking = Animator.StringToHash("isAttacking");

    public string[] AnimationStrings;

    public float NavAgentSpeed;

    public override void Start()
    {
        base.Start();

        NavMeshAgent = GetComponent<NavMeshAgent>();

        NavAgentSpeed = NavMeshAgent.speed;

        SkillSetup();
    }

    public void SkillSetup()
    {
        ActiveSkills = new Skill[UsedSkills.Length];

        for (int i = 0; i < UsedSkills.Length; i++)
        {
            Skill curSkill = Instantiate(UsedSkills[i], transform.position + UsedSkills[i].transform.position, Quaternion.identity);
            curSkill.transform.SetParent(transform);
            curSkill.Character = this;
            curSkill.SkillSpawn = SkillSpawn;
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
        else
        {
            NavMeshAgent.enabled = false;
        }
    }

    public override void Update()
    {
        Anim.SetFloat(animSpeed, NavMeshAgent.velocity.magnitude / NavAgentSpeed);

        if (!isGameOver)
        {
            base.Update();

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

                                if (i < AnimationStrings.Length)
                                {
                                    if (AnimationStrings[i] != null)
                                    {
                                        Anim.SetTrigger(animIsAttacking);
                                    }
                                }
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

    bool GetNearestTargetWithNavMesh()
    {
        PlayerInRadius.Clear();

        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            Transform tempPlayer = GameManager.Instance.Players[i].GetComponent<Transform>();

            if (CheckIsAlive(tempPlayer))
            {
                PlayerInRadius.Add(tempPlayer);
            }
        }

        int NearestPlayer = 0;
        float ShortestWay = 0;
        NavMeshPath Path = new NavMeshPath();
        bool PlayersInRange = false;

        if (PlayerInRadius.Count > 0)
        {
            for (int i = 0; i < PlayerInRadius.Count; i++)
            {
                float LenghtSoFar = 0f;

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
        if (Vector3.Distance(transformTarget.position, transform.position) > MaxRange)
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

        var lookPos = new Vector3(Target.position.x, 0, Target.position.z) - new Vector3(transform.position.x, 0, transform.position.z);
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * LookDamping);

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
            Transform tempPlayer = GameManager.Instance.Players[i].GetComponent<Transform>();

            if (CheckIsAlive(tempPlayer))
            {
                PlayerInRadius.Add(tempPlayer);
            }
        }
    }
}