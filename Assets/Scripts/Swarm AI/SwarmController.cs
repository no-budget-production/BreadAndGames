using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmController : Character
{
    public float PlayerStopDistance;

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
            MyNavMeshAgent.stoppingDistance = 0;
            Vector3 targetVector = Destination.transform.position;
            MyNavMeshAgent.SetDestination(targetVector);
        }
    }

    public void AttackCommand(Transform Destination)
    {
        if (!isDeadTrigger)
        {
            MyNavMeshAgent.stoppingDistance = PlayerStopDistance;
            Vector3 targetVector = Destination.transform.position;
            MyNavMeshAgent.SetDestination(targetVector);
        }
    }
}
