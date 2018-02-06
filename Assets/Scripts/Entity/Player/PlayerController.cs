﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ButtonConfig
{
    public Skill SkillBC;
    public ButtonString ButtonStringBC;
}

public class PlayerController : Character
{
    public int usedButtonsCount;
    public int[] usedButtons;
    public bool[] areButtons;
    public float[] deadZones;
    public string[] thisPlayerString;

    public string PlayerNumber;

    public bool isWalking;
    public bool isUsingRightStick;

    public ButtonConfig[] PlayerSkills;

    public Skill[] ActiveSkills;

    [Range(0.0f, 1.0f)]
    public float TurnSpeed;



    public float moveSpeed = 1.0f;
    public float deadzone = 0.25f;
    //public float deadzoneMove = 0.25f;


    private float gravityStrength = 15f;
    private float angle;

    public float acceleration = 1.2f;

    public Vector3 temporaryVector;
    public Vector3 temporaryLookVector;

    public bool playAttackAnim;

    public Animator animator;
    public CharacterController myController;

    private Vector3 currentMovement;
    private Vector3 moveVector;
    private Vector3 lookVector;

    private Quaternion inputRotation;


    //Müll

    //public Collider PlayerTrigger;
    //public GunSystem gunSystem;
    //public Transform Camera;
    //public Camera viewCamera;

    protected override void Start()
    {
        base.Start();
        ButtonSetup();
        //PlayerTrigger = GetComponent<Collider>();
        //gunSystem = GetComponent<GunSystem>();
    }

    public void Setup(Quaternion inputRotationArg, string[] buttonStrings)
    {
        inputRotation = inputRotationArg;
        int tempLength = buttonStrings.Length;
        thisPlayerString = new string[tempLength];
        for (int i = 0; i < tempLength; i++)
        {
            thisPlayerString[i] = buttonStrings[i] + PlayerNumber;
        }
    }

    public void ButtonSetup()
    {
        usedButtonsCount = PlayerSkills.Length;
        usedButtons = new int[usedButtonsCount];
        areButtons = new bool[usedButtonsCount];
        deadZones = new float[usedButtonsCount];
        ActiveSkills = new Skill[usedButtonsCount];
        for (int i = 0; i < usedButtonsCount; i++)
        {
            usedButtons[i] = PlayerSkills[i].ButtonStringBC.ButtonID;
            areButtons[i] = PlayerSkills[i].ButtonStringBC.isButton;
            deadZones[i] = PlayerSkills[i].ButtonStringBC.DeadZone;

            Skill curSkill = Instantiate(PlayerSkills[i].SkillBC, transform.position + PlayerSkills[i].SkillBC.transform.position, Quaternion.identity);
            curSkill.transform.SetParent(transform);
            ActiveSkills[i] = curSkill;
        }
    }

    private void Move()
    {
        //inputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Camera.forward, Vector3.up)); // align movement to camera view (can put in start() if camera view doesnt change)

        float Horizontal_PX = Input.GetAxis(thisPlayerString[0]);
        float Vertical_PX = Input.GetAxis(thisPlayerString[1]);
        float HorizontalLook_PX = Input.GetAxis(thisPlayerString[2]);
        float VerticalLook_PX = Input.GetAxis(thisPlayerString[3]);

        if (Horizontal_PX != 0 || Vertical_PX != 0)
        {
            moveVector = new Vector3(0, 0, 0);
            lookVector = new Vector3(0, 0, 0);

            // XBox (left stick) movement input
            moveVector.x += Horizontal_PX * acceleration;
            temporaryVector.x = Horizontal_PX;
            moveVector.z += Vertical_PX * acceleration;
            temporaryVector.z = Vertical_PX;

            moveVector = Vector3.ClampMagnitude(moveVector, 0.5f);
            moveVector = moveVector * moveSpeed * Time.deltaTime;
            moveVector = inputRotation * moveVector;

            isWalking = true;
        }
        else
        {
            isWalking = false;
        }


        if (HorizontalLook_PX != 0 || VerticalLook_PX != 0)
        {
            // XBox (right stick) look input
            lookVector.z += Input.GetAxis(thisPlayerString[2]);
            temporaryLookVector.x = Input.GetAxis(thisPlayerString[2]);
            lookVector.x += Input.GetAxis(thisPlayerString[3]);
            temporaryLookVector.z = Input.GetAxis(thisPlayerString[3]);

            isUsingRightStick = true;

            lookVector = inputRotation * lookVector;

            if (lookVector.magnitude < deadzone)
            {
                lookVector = Vector3.zero;
            }

            if (lookVector.x != 0 || lookVector.z != 0)
            {
                transform.rotation = Quaternion.LookRotation(lookVector);
            }
            else if (moveVector.x != 0 || moveVector.z != 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveVector), TurnSpeed);
            }

        }
        else
        {
            isUsingRightStick = false;
        }


        //moves the character
        if (isWalking)
        {
            CollisionFlags flags = myController.Move(moveVector);
        }

    }

    private void CheckButtonInput()
    {
        for (int i = 0; i < usedButtonsCount; i++)
        {
            if (areButtons[i])
            {
                if (Input.GetButtonDown(thisPlayerString[usedButtons[i]]))
                {
                    ActiveSkills[i].Shoot();
                    Debug.Log("ShootButton" + " isButton" + areButtons[i] + " PlayerString" + thisPlayerString[usedButtons[i]]);
                }
            }
            else
            {
                if (Input.GetAxis(thisPlayerString[usedButtons[i]]) < deadZones[i])
                {
                    ActiveSkills[i].Shoot();
                    Debug.Log("ShootTriggern" + " isButton" + areButtons[i] + " PlayerString" + thisPlayerString[usedButtons[i]]);
                }
            }
        }
    }

    void Update()
    {

        Move();
        CheckButtonInput();
    }
}