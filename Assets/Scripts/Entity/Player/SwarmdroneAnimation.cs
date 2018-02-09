using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmdroneAnimation : MonoBehaviour
{
    public Vector3 debugVec;

    private Animator animator;
    private NavMeshAgent navMeshAgent;

	void Awake ()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
	

	void Update ()
    {
        debugVec = navMeshAgent.velocity;
	}
}
