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

    protected override void Start()
    {
        base.Start();
    }

    //private void SetDestination()
    //{
    //    if (Destination != null)
    //    {
    //        Vector3 targetVector = Destination.transform.position;
    //        NavMeshAgent.SetDestination(targetVector);
    //    }
    //}

    public void MoveToHoldingPoint(Transform Destination)
    {
        Vector3 targetVector = Destination.transform.position;
        NavMeshAgent.SetDestination(targetVector);
    }
}
