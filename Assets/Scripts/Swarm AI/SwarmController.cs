﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmController : Entity
{
    private NavMeshAgent MyNavMeshAgent;
    private bool DoItOnceBool;
    private SwarmController MySwarmController;

    private SwarmCluster SwarmClusterScript;
    public SwarmCluster _SwarmClusterScript { get { return SwarmClusterScript; } set { SwarmClusterScript = value; } }

    private int IndexNumber;
    public int _IndexNumber { get { return IndexNumber; } set { IndexNumber = value; } }

    void Awake()
    {
        MyNavMeshAgent = this.GetComponent<NavMeshAgent>();
        MyNavMeshAgent.speed = 15f;
        MyNavMeshAgent.angularSpeed = 200;

        DoItOnceBool = true;
        MySwarmController = GetComponent<SwarmController>();
    }

    void Update()
    {
        if (isDeadTrigger && DoItOnceBool)
        {
            DoItOnceBool = false;
            SwarmClusterScript._AllEnemysInCluster.Remove(this.gameObject);
            SwarmClusterScript._SwarmControllerScripts.Remove(this.MySwarmController);
            if (DestroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }



    public void MoveToDestination(Transform Destination)
    {
        if (!isDeadTrigger)
        {
            Vector3 targetVector = Destination.transform.position;
            MyNavMeshAgent.SetDestination(targetVector);
        }
    }
}
