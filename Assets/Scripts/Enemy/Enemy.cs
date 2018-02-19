using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    private NavMeshAgent NavMeshAgent;

    public List<Transform> PlayerInRadius = new List<Transform>(3);
    //public List<Transform> PlayerInView = new List<Transform>(3);

    public List<Transform> PlayersInRange = new List<Transform>(3);

    private Transform Target;


    private bool isInRange;
    private bool isTargetInView;
    private bool ísSooting;

    public float MaxShootingRange;
    public float StopRange;

    public Skill[] UsedSkills;

    private Vector3 direction;

    public int EveryXFramesDistanceCheck;
    private int FrameCounterDistanceCheck;
    public int EveryXFramesRayCheck;
    private int FrameCounterRayCheck;
    public int EveryXFramesShoot;
    private int FrameCounterShoot;
    public int EveryXFramesFind;
    private int FrameCounterFind;


    public override void Start()
    {
        base.Start();

        NavMeshAgent = GetComponent<NavMeshAgent>();

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

        GetNearestTargetWithNavMesh();
    }

    public override void Update()
    {
        base.Update();

        FrameCounterDistanceCheck++;
        if ((FrameCounterDistanceCheck % EveryXFramesDistanceCheck) == 0)
        {
            isInRange = InRange(Target, MaxShootingRange);
        }

        if (isInRange)
        {
            FrameCounterRayCheck++;
            if ((FrameCounterRayCheck % EveryXFramesRayCheck) == 0)
            {
                LockOn();
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

                    ísSooting = true;
                    NavMeshAgent.enabled = false;
                }
            }
            else
            {
                NavMeshAgent.enabled = true;

                if ((FrameCounterFind % EveryXFramesFind) == 0)
                {
                    GetNearestTargetWithNavMesh();
                }

                ísSooting = false;
            }
        }
        else
        {
            ísSooting = false;
        }

        if (!ísSooting)
        {
            NavMeshAgent.destination = Target.position;
        }
    }

    void GetNearestTargetWithNavMesh()
    {
        PlayerInRadius.Clear();

        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            Transform tempPlayer = GameManager.Instance.Players[i].GetComponent<Transform>();

            PlayerInRadius.Add(tempPlayer);

        }

        int NearestPlayer = 0;
        float ShortestWay = 0;
        NavMeshPath Path = new NavMeshPath();
        bool PlayersInRange = false;

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
        direction = Target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(Target.rotation, lookRotation, Time.deltaTime * 100000000).eulerAngles;
        Target.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    bool CheckView()
    {
        RaycastHit hit;

        if (Physics.Raycast(Target.position, direction, out hit))
        {
            PlayerController temp = hit.collider.GetComponent<PlayerController>();
            if (temp == null)
            {
                return true;
            }

            if (temp.transform == Target)
            {
                return true;
            }
        }
        return false;
    }
}
