﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public CharacterController myController;
    public Collider PlayerTrigger;
    public Transform playerCamera;
    GunSystem gunSystem;

    public float moveSpeed = 1.0f;

    private float gravityStrength = 15f;

    private float angle;
    private float deadzone = 0.25f;

    private Transform CurrentTransform;

    Vector3 currentMovement;

    public float acceleration = 1.2f;

    public Vector3 temporaryVector;
    public Vector3 temporaryLookVector;


    void Start()
    {
        PlayerTrigger = GetComponent<Collider>();
        gunSystem = GetComponent<GunSystem>();
        CurrentTransform = playerCamera;
    }

    void Update()
    {
        //Movement Input
        Vector3 myVector = new Vector3(0, 0, 0);
        Vector3 lookVector = new Vector3(0, 0, 0);

            myVector.x += Input.GetAxis("HorizontalXbox1") * acceleration;
            temporaryVector.x = Input.GetAxis("HorizontalXbox1");
            myVector.z += Input.GetAxis("VerticalXbox1") * acceleration;
            temporaryVector.z = Input.GetAxis("VerticalXbox1");
            myVector = Vector3.ClampMagnitude(myVector, 3.0f);
            myVector = myVector * moveSpeed * Time.deltaTime;

            Quaternion inputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(CurrentTransform.forward, Vector3.up));
            myVector = inputRotation * myVector;

        lookVector.z += Input.GetAxis("HorizontalLook1");
        temporaryLookVector.x = Input.GetAxis("HorizontalLook1");
        lookVector.x += Input.GetAxis("VerticalLook1");
        temporaryLookVector.z = Input.GetAxis("VerticalLook1");
        lookVector = inputRotation * lookVector;

        if (lookVector.magnitude < deadzone)
        {
            lookVector = Vector3.zero;
        }

        //moves the character
        CollisionFlags flags = myController.Move(myVector);

        if (myVector.x != 0 || myVector.z != 0)
        {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(myVector), 0.5f);
        } 
        if(lookVector.x != 0 || lookVector.z != 0)
        {
            transform.rotation = Quaternion.LookRotation(lookVector);
        }

        //Weapon Input
        if(Input.GetAxis("FireXbox1") < -0.25f)
        {
            gunSystem.Shoot();
        }

    }
}
