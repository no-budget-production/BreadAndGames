using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmMove : MonoBehaviour {

    public Transform Destination;

    private NavMeshAgent NavMeshAgent;

	void Awake ()
    {
        NavMeshAgent = this.GetComponent<NavMeshAgent>();
		
        if(NavMeshAgent == null)
        {
            Debug.LogError("The NavMeshAgent component ist not attached to " + gameObject.name);
        }
        else
        {
            SetDestination();
        }
	}

    private void SetDestination()
    {
        if (Destination != null)
        {
            Vector3 targetVector = Destination.transform.position;
            NavMeshAgent.SetDestination(targetVector);
        }
    }

}
