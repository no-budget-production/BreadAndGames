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

    public float MeleeDamage = 1f;
    public float RangeDamage = 1f;
    public float Accuracy = 1f;

    public float MeleeDamageMultiplicator = 1f;
    public float RangeDamageMultiplicator = 1f;
    public float AccuracyMultiplicator = 1f;

    public bool canWalk = true;
    public bool canUseRightStick = true;
    public bool canUseSkills = true;

    public bool isWalking;
    public bool isUsingRightStick;

    public ButtonConfig[] PlayerSkills;

    public Skill[] ActiveSkills;

    public List<Buff> ActiveBuffs;

    public bool isInAction;
    public int ActionPoints;
    public int curActionPoints;

    public Transform SkillSpawn;
    public Transform TakeHitPoint;

    public float ActionCD;

    [Range(0.0f, 1.0f)]
    public float TurnSpeed;

    public float MoveSpeed;
    public float MoveSpeedMultiplicator = 1f;

    public float Deadzone;

    private float angle;

    public float Acceleration;

    public Vector3 temporaryVector;
    public Vector3 temporaryLookVector;

    public bool playAttackAnim;

    public Animator animator;
    public CharacterController myController;

    private Vector3 currentMovement;
    //private Vector3 moveVector;
    //private Vector3 lookVector;

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

        //Debug.Log("BumpTriggers");
        //thisPlayerString[4] = GameManager.Instance.prefabLoader.ButtonStrings[4] + (playerNumber + 1);
        //thisPlayerString[5] = GameManager.Instance.prefabLoader.ButtonStrings[5] + (playerNumber + 1);
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
        Vector3 moveVector = new Vector3(Horizontal_PX, 0, Vertical_PX);
        Vector3 lookVector = new Vector3(VerticalLook_PX, 0, HorizontalLook_PX);

        //moveVector = new Vector3(0, 0, 0);
        //lookVector = new Vector3(0, 0, 0);

        if (canWalk)
        {

            //if (((Mathf.Abs(Horizontal_PX)) > deadZones[0]) || ((Mathf.Abs(Vertical_PX)) > deadZones[1]))
            if (moveVector.magnitude > deadZones[0])
            {
                isWalking = true;
                moveVector = inputRotation * moveVector;

                // XBox (left stick) movement input
                //moveVector.x += moveVector2.x;
                //temporaryVector.x = Horizontal_PX;
                //moveVector.z += moveVector2.y;
                //temporaryVector.z = Vertical_PX;
                currentMovement += moveVector * Acceleration;
                float speed = currentMovement.magnitude;
                if (speed > MoveSpeed)
                {
                    currentMovement *= MoveSpeed / speed;
                }

            }
            else
            {
                currentMovement = Vector3.zero;
                isWalking = false;
            }
        }


        if (canUseRightStick)
        {
            //if ((Mathf.Abs(HorizontalLook_PX) != 0) || ((Mathf.Abs(VerticalLook_PX) != 0)))
            if ((lookVector.magnitude > deadZones[0]))
            {
                isUsingRightStick = true;


                lookVector = inputRotation * lookVector;

                //if (lookVector.magnitude < deadzone)
                //{
                //    lookVector = Vector3.zero;
                //}

            }
            else
            {
                isUsingRightStick = false;
            }
        }

        //if (Horizontal_PX != 0 || Vertical_PX != 0)
        //{
        //    isWalking = true;
        //}
        //else
        //{
        //    isWalking = false;
        //}

        //if (HorizontalLook_PX != 0 || VerticalLook_PX != 0)
        //{
        //    isUsingRightStick = true;
        //}
        //else
        //{
        //    isUsingRightStick = false;
        //}

        if (isUsingRightStick)
        {
            //transform.rotation = Quaternion.LookRotation(lookVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookVector), 0.35f);
        }
        else if (isWalking)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveVector), TurnSpeed);
        }

        //moves the character
        if (isWalking)
        {
            myController.Move(currentMovement * Time.deltaTime);
        }

    }

    /*public void Walk()
    {
        CollisionFlags flags = myController.Move(moveVector);
    }*/

    private void CheckButtonInput()
    {
        if (canUseSkills)
        {
            int tempIJ = 0;
            for (int i = 0; i < PlayerSkills.Length; i++)
            {
                bool tempIsShooting = false;
                for (int j = 0; j < PlayerSkills[i].ButtonStringBC.Length; j++)
                {
                    if (areButtons[tempIJ])
                    {
                        if (Input.GetButtonDown(thisPlayerString[usedButtons[tempIJ]]))
                        {
                            ActiveSkills[i].Shoot();
                            ActiveSkills[i].isFiring = true;
                            tempIsShooting = true;
                            //DebugConsole.Log(this.gameObject.name + " " + PlayerNumber + " Joystick " + Input.GetJoystickNames() + " ShootButton" + " isButton" + areButtons[i] + " PlayerString" + thisPlayerString[usedButtons[i]]);
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(Input.GetAxis(thisPlayerString[usedButtons[tempIJ]])) > deadZones[tempIJ])
                        {
                            ActiveSkills[i].Shoot();
                            ActiveSkills[i].isFiring = true;
                            tempIsShooting = true;
                            //DebugConsole.Log(this.gameObject.name + " " + PlayerNumber + " Joystick " + Input.GetJoystickNames() + " ShootTriggern" + " isButton" + areButtons[i] + " PlayerString" + thisPlayerString[usedButtons[i]]);
                        }
                    }
                    tempIJ++;
                }
                if (!tempIsShooting)
                {
                    ActiveSkills[i].isFiring = false;
                    ActiveSkills[i].StopShoot();
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
