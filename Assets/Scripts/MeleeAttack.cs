using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public Animator animator;

    void Awake()
    {
        //animator = gameObject.GetComponent<Animator>();
    }

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            animator.SetTrigger("Button 0");
        }
    }
}
