using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newShootHook : MonoBehaviour
{

    private Transform whoShootTransform;
    public GameObject hookLeaderPrefab;
    private CharacterController characterController;
    public static GameObject hookLeaderObject;

    private float shootTime;
    public float shootInterval = 0.2f;

    private Transform thisTransform;

    void Start ()
    {
        thisTransform = transform;
        whoShootTransform = thisTransform.parent;
        characterController = whoShootTransform.gameObject.GetComponent<CharacterController>();
	}
	
    //check for Input
	void FixedUpdate ()
    {
		if(Input.GetButton("LeftBumper"))
        {
            Shooting(thisTransform.rotation);
        }
	}

    // Shooting the hook
    private void Shooting(Quaternion hookRotation)
    {
        if (hookLeaderObject != null || Time.time - shootTime < shootInterval)
        {
            return;
        }

        shootTime = Time.time;
        hookLeaderObject = (GameObject)Instantiate(hookLeaderPrefab, thisTransform.position, hookRotation);
        HookLeader hookLeader = hookLeaderObject.GetComponent<HookLeader>();
        //hookLeader.Init(thisTransform);
    }
}
