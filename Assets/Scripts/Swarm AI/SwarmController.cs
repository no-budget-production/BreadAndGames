using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmController : Entity
{
    private NavMeshAgent NavMeshAgent;
    private SwarmCluster SwarmClusterScript;
    public SwarmCluster _SwarmClusterScript { get { return SwarmClusterScript; } set { SwarmClusterScript = value; }
    }

    void Awake()
    {
        NavMeshAgent = this.GetComponent<NavMeshAgent>();
        NavMeshAgent.speed = 15f;
        NavMeshAgent.angularSpeed = 200;
    }


    
    public void MoveToDestination(Transform Destination)
    {
        Vector3 targetVector = Destination.transform.position;
        NavMeshAgent.SetDestination(targetVector);
    }
}
