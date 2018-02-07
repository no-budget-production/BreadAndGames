using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    Animator animator;

    public Vector3 moveVector;
    public Vector3 lookVector;

    // Use this for initialization
    void Awake ()
    {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {

        Vector3 moveVector = new Vector3(0, 0, 0);
        Vector3 lookVector = new Vector3(0, 0, 0);

        moveVector.x += Input.GetAxis("LeftStickX1");
        moveVector.y += Input.GetAxis("LeftStickY1");

        lookVector.z += Input.GetAxis("RightStickZ1");
        lookVector.x += Input.GetAxis("RightStickX1");


        animator.SetFloat("MovX", moveVector.magnitude);

    }
}
