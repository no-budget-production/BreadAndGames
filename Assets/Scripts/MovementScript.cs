﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public int playerNumber;
    public bool XboxController;

    public string movementAxisHorizontalName;
    public string movementAxisVerticalName;

    public string movementAxisHorizontalXboxName;
    public string movementAxisVerticalXboxName;
    public string movementAxisHorizontalLookXboxName;
    public string movementAxisVerticalLookXboxName;
    public string FireXbox;

    [Range(0.0f, 1.0f)]
    public float TurnSpeed;

    // Alex Start

    public CharacterController myController;
    public Collider PlayerTrigger;
    public Transform Camera;
    public Camera viewCamera;
    public EnemyTurret Turret;

    public float moveSpeed = 1.0f;

    private float gravityStrength = 15f;

    private float angle;
    public float deadzone = 0.25f;
    //public float deadzoneMove = 0.25f;

    Vector3 currentMovement;

    public float acceleration = 1.2f;

    public Vector3 temporaryVector;
    public Vector3 temporaryLookVector;

    public bool playAttackAnim;
    public Animator animator;

    //protected override void Start()
    //{
    //    base.Start();
    //    PlayerTrigger = GetComponent<Collider>();
    //    //gunSystem = GetComponent<GunSystem>();
    //}

    public void Awake()
    {
        SetInput();
    }

    public void SetInput()
    {
        movementAxisHorizontalName += playerNumber;
        movementAxisVerticalName += playerNumber;

        movementAxisHorizontalXboxName += playerNumber;
        movementAxisVerticalXboxName += playerNumber;

        movementAxisHorizontalLookXboxName += playerNumber;
        movementAxisVerticalLookXboxName += playerNumber;

        FireXbox += playerNumber;
    }

    void Update()
    {
        Vector3 moveVector = new Vector3(0, 0, 0);
        Vector3 lookVector = new Vector3(0, 0, 0);

        //Movement Input PC
        if (XboxController)
        {
            //Movement Input Xbox
            moveVector.x += Input.GetAxis(movementAxisHorizontalXboxName) * acceleration;
            temporaryVector.x = Input.GetAxis(movementAxisHorizontalXboxName);
            moveVector.z += Input.GetAxis(movementAxisVerticalXboxName) * acceleration;
            temporaryVector.z = Input.GetAxis(movementAxisVerticalXboxName);
        }
        else
        {
            //Movement Input PC
            moveVector.x += Input.GetAxis(movementAxisHorizontalXboxName) * acceleration;
            temporaryVector.x = Input.GetAxis(movementAxisHorizontalXboxName);
            moveVector.z += Input.GetAxis(movementAxisVerticalXboxName) * acceleration;
            temporaryVector.z = Input.GetAxis(movementAxisVerticalXboxName);
        }

        Quaternion inputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Camera.forward, Vector3.up)); // align movement to camera view [can put in start() if camera view doesnt change]

        moveVector = Vector3.ClampMagnitude(moveVector, 0.5f);
        moveVector = moveVector * moveSpeed * Time.deltaTime;
        moveVector = inputRotation * moveVector;

        //Look Input Xbox
        lookVector.z += Input.GetAxis(movementAxisHorizontalLookXboxName);
        temporaryLookVector.x = Input.GetAxis(movementAxisHorizontalLookXboxName);
        lookVector.x += Input.GetAxis(movementAxisVerticalLookXboxName);
        temporaryLookVector.z = Input.GetAxis(movementAxisVerticalLookXboxName);

        lookVector = inputRotation * lookVector;
        

        if (lookVector.magnitude < deadzone)
        {
            lookVector = Vector3.zero;
        }

        //Look Input Mouse **BUGGY**
        /*
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

         if (groundPlane.Raycast(ray, out rayDistance))
         {
             Vector3 lookPoint = ray.GetPoint(rayDistance);
             Debug.DrawLine(ray.origin, lookPoint, Color.red);
             Debug.DrawRay(ray.origin,ray.direction * 100,Color.red);
             Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
             transform.LookAt(heightCorrectedPoint);
         }*/

        //moves the character
        CollisionFlags flags = myController.Move(moveVector);

        if (moveVector.x != 0 || moveVector.z != 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveVector), TurnSpeed);
            //transform.rotation = Quaternion.LookRotation(moveVector);
        } 
        if(lookVector.x != 0 || lookVector.z != 0)
        {
            transform.rotation = Quaternion.LookRotation(lookVector);
        }

        //Weapon Input
        if (Input.GetAxis(FireXbox) < -0.25f)
        {
            Turret.Shoot();

        }
        //else if (Input.GetButton("FirePC1"))
        //{
        //    gunSystem.Shoot();
        //}
    }
   
}
