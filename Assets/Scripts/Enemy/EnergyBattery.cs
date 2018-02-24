using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnergyBattery : Character
{
    [Header(">>>>>>>>>> EnergyBattery:")]


    public List<Transform> PlayerInRadius = new List<Transform>();

    public List<Transform> PlayersInRange = new List<Transform>();

    public Transform Target;

    public Skill[] UsedSkills;

    public bool isGameOver;
    public bool isAlive;
    public bool isInRange;
    //public bool isTargetInView;
    public bool isSooting;

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

    public float NavAgentSpeed;

    public override void Start()
    {
        base.Start();

        SkillSetup();

        GetNearestTargetWithNavMesh();
    }

    public virtual void OnDestroy()
    {
        base.OnDestroy();
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
    }

    public override void Update()
    {
        if (!isGameOver)
        {
            base.Update();

            for (int i = 0; i < ActiveSkills.Length; i++)
            {
                ActiveSkills[i].Shoot();
            }

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
                    //FrameCounterRayCheck++;
                    //if ((FrameCounterRayCheck % EveryXFramesRayCheck) == 0)
                    //{
                    //    //LockOn();
                    //    isTargetInView = CheckView();
                    //    FrameCounterRayCheck = 0;
                    //}

                    //if (isTargetInView)
                    //{

                    //}
                    //else
                    //{
                    //    FrameCounterFind++;
                    //    if ((FrameCounterFind % EveryXFramesFind) == 0)
                    //    {
                    //        GetNearestTargetWithNavMesh();
                    //        FrameCounterFind = 0;
                    //    }

                    //    isSooting = false;
                    //}
                }
                else
                {
                    //isTargetInView = false;
                    isSooting = false;
                }

                if (!isSooting)
                {
                    //FrameCounterFind++;
                    //if ((FrameCounterFind % EveryXFramesFind) == 0)
                    //{
                    //    if (GetNearestTargetWithNavMesh())
                    //    {
                    //    }
                    //    else
                    //    {
                    //        isGameOver = true;
                    //    }
                    //    FrameCounterFind = 0;
                    //}


                }
                else
                {
                    if (GetNearestTargetWithNavMesh())
                    {
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
                }
                else
                {
                    isGameOver = true;
                }

            }
        }
    }

    //bool CheckView()
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, direction, out hit))
    //    {
    //        var temp = hit.collider.GetComponent<PlayerController>();
    //        if (temp != null)
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }
    //    return false;
    //}

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
        float ShortestWay = MaxShootingRange;

        if (PlayerInRadius.Count > 0)
        {
            for (int i = 0; i < PlayerInRadius.Count; i++)
            {
                float curLength = 0f;

                if (PlayerInRadius[i] != null)
                {
                    curLength = Vector3.Distance(PlayerInRadius[i].transform.position, transform.position);

                    if (curLength < ShortestWay)
                    {
                        ShortestWay = curLength;
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
        var lookPos = new Vector3(Target.position.x, 0, Target.position.z) - new Vector3(transform.position.x, 0, transform.position.z);
        lookPos.y = 0;
        if (lookPos != Vector3.zero)
        {
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * LookDamping);
        }
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