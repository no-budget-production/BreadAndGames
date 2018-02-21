using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAnimation : MonoBehaviour
{

    Animator animator;

    Vector3 moveVector;
    Vector3 lookVector;

    // Use this for initialization
    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isAiming", true);
        animator.SetBool("isRunning", false);
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 moveVector = new Vector3(0, 0, 0);
        Vector3 lookVector = new Vector3(0, 0, 0);

        moveVector.x += Input.GetAxis("Horizontal_PX2");
        moveVector.y += Input.GetAxis("Vertical_PX2");

        lookVector.x += Input.GetAxis("HorizontalLook_PX2");
        lookVector.y += Input.GetAxis("VerticalLook_PX2");

        if (lookVector.magnitude <= 0.11f)
        {
            //Debug.Log("isRunning");
            animator.SetBool("isRunning", true);
            animator.SetFloat("MovX", moveVector.magnitude);
        }
        else if (lookVector.magnitude >= 0f)
        {

            //Debug.Log("isAiming");
            animator.SetBool("isAiming", true);
            animator.SetBool("isRunning", false);
            animator.SetFloat("Aim_Amount", lookVector.magnitude);
            animator.SetFloat("MovY", Mathf.Clamp(moveVector.y, -1, 0.5f));
            animator.SetFloat("MovX", moveVector.x);
        }

        if(Input.GetButtonDown("Bumper_Left_PX"))
        {
            animator.SetBool("isAttacking", true);
            animator.SetBool("leftPunch", true);
        }
        else if (Input.GetButtonUp("Bumper_Left_PX"))
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("leftPunch", false);
        }
        if (Input.GetButtonDown("Bumper_Right_PX"))
        {
            animator.SetBool("isAttacking", true);
            animator.SetBool("rightPunch", true);
        }
        else if (Input.GetButtonUp("Bumper_Right_PX"))
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("rightPunch", false);
        }


    }
}
