﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ButtonConfig
{
    public Skill SkillBC;
    public ButtonString[] ButtonStringBC;
}

public class PlayerController : Character
{
    public PlayerType Type;

    public bool HasFlag(PlayerTypeFlags flags)
    {
        int typeflag = 1 << (int)Type;
        return (typeflag & (int)flags) != 0;
    }

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

    public bool isInAction;
    public int ActionPoints;
    public int curActionPoints;

    public Transform SkillSpawn;

    public float ActionCD;

    [Range(0.0f, 1.0f)]
    public float TurnSpeed;

    public float moveSpeed = 1.0f;
    public float deadzone = 0.25f;

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

    //Dump

    //public float deadzoneMove = 0.25f;
    //private float gravityStrength = 15f;

    protected override void Start()
    {
        base.Start();
        ButtonSetup();
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
        for (int i = 0; i < PlayerSkills.Length; i++)
        {
            usedButtonsCount += PlayerSkills[i].ButtonStringBC.Length;
        }

        usedButtons = new int[usedButtonsCount];
        areButtons = new bool[usedButtonsCount];
        deadZones = new float[usedButtonsCount];

        ActiveSkills = new Skill[PlayerSkills.Length];

        int tempIJ = 0;
        for (int i = 0; i < PlayerSkills.Length; i++)
        {
            Skill curSkill = Instantiate(PlayerSkills[i].SkillBC, transform.position + PlayerSkills[i].SkillBC.transform.position, Quaternion.identity);
            curSkill.transform.SetParent(transform);
            curSkill.Player = this.gameObject;
            curSkill.PlayerController = this;
            curSkill.SkillSpawn = SkillSpawn;
            ActiveSkills[i] = curSkill;

            for (int j = 0; j < PlayerSkills[i].ButtonStringBC.Length; j++)
            {
                usedButtons[tempIJ] = PlayerSkills[i].ButtonStringBC[j].ButtonID;
                areButtons[tempIJ] = PlayerSkills[i].ButtonStringBC[j].isButton;
                deadZones[tempIJ] = PlayerSkills[i].ButtonStringBC[j].DeadZone;
                tempIJ++;
            }

        }
    }

    private void Move()
    {
        float Horizontal_PX = Input.GetAxis(thisPlayerString[0]);
        float Vertical_PX = Input.GetAxis(thisPlayerString[1]);
        float HorizontalLook_PX = Input.GetAxis(thisPlayerString[2]);
        float VerticalLook_PX = Input.GetAxis(thisPlayerString[3]);

        //if (Horizontal_PX != 0 || Vertical_PX != 0 || HorizontalLook_PX != 0 || VerticalLook_PX != 0)
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
        }

        {
            // XBox (right stick) look input
            lookVector.z += Input.GetAxis(thisPlayerString[2]);
            temporaryLookVector.x = Input.GetAxis(thisPlayerString[2]);
            lookVector.x += Input.GetAxis(thisPlayerString[3]);
            temporaryLookVector.z = Input.GetAxis(thisPlayerString[3]);

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

        if (Horizontal_PX != 0 || Vertical_PX != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        if (HorizontalLook_PX != 0 || VerticalLook_PX != 0)
        {
            isUsingRightStick = true;
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
        int tempIJ = 0;
        for (int i = 0; i < PlayerSkills.Length; i++)
        {
            for (int j = 0; j < PlayerSkills[i].ButtonStringBC.Length; j++)
            {
                if (areButtons[tempIJ])
                {
                    if (Input.GetButtonDown(thisPlayerString[usedButtons[tempIJ]]))
                    {
                        ActiveSkills[i].Shoot();
                        ActiveSkills[i].isFiring = true;
                        //Debug.Log("ShootButton" + " isButton" + areButtons[i] + " PlayerString" + thisPlayerString[usedButtons[i]]);
                    }
                    else
                    {
                        ActiveSkills[i].isFiring = false;
                    }
                }
                else
                {

                    if (Input.GetAxis(thisPlayerString[usedButtons[tempIJ]]) > deadZones[tempIJ])
                    {
                        ActiveSkills[i].Shoot();
                        ActiveSkills[i].isFiring = true;
                        //Debug.Log("ShootTriggern" + " isButton" + areButtons[i] + " PlayerString" + thisPlayerString[usedButtons[i]]);
                    }
                    else
                    {
                        ActiveSkills[i].isFiring = false;
                    }
                }
                tempIJ++;
            }
        }
    }

    void Update()
    {
        Move();
        CheckButtonInput();
    }
}
