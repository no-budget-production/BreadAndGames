using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmController : Entity
{
    private NavMeshAgent NavMeshAgent;
    private bool OneFrameBool;

    private SwarmCluster SwarmClusterScript;
    public SwarmCluster _SwarmClusterScript { get { return SwarmClusterScript; } set { SwarmClusterScript = value; } }

    private int IndexNumber;
    public int _IndexNumber { get { return IndexNumber; } set { IndexNumber = value; } }
    
    void Awake()
    {
        NavMeshAgent = this.GetComponent<NavMeshAgent>();
        NavMeshAgent.speed = 15f;
        NavMeshAgent.angularSpeed = 200;

        OneFrameBool = true;
    }

    void Update()
    {
        if (IsDeadTrigger && OneFrameBool)
        {
            OneFrameBool = false;
            //_SwarmClusterScript._EnemysInCluster[_IndexNumber] =
        }
    }

    public void MoveToDestination(Transform Destination)
    {
        Vector3 targetVector = Destination.transform.position;
        NavMeshAgent.SetDestination(targetVector);
    }
}
