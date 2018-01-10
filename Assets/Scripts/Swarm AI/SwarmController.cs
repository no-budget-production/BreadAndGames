using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmController : Entity
{
    private NavMeshAgent NavMeshAgent;

    void Awake()
    {
        NavMeshAgent = this.GetComponent<NavMeshAgent>();

    }
    
    public void MoveToDestination(Transform Destination)
    {
        Vector3 targetVector = Destination.transform.position;
        NavMeshAgent.SetDestination(targetVector);
    }
}
