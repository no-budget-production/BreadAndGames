using System.Collections;
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
    public bool isInAction;

    public ButtonConfig[] PlayerSkills;

    [Range(0.0f, 1.0f)]
    public float TurnSpeed;

    public float Deadzone;

    private float angle;

    public float Acceleration;

    public Vector3 temporaryVector;
    public Vector3 temporaryLookVector;

    public Animator Anim;

    int animMovX = Animator.StringToHash("MovX");
    int animMovY = Animator.StringToHash("MovY");
    int animIsAiming = Animator.StringToHash("isAiming");
    int animIsRunning = Animator.StringToHash("isRunning");
    int animIsAim_Amount = Animator.StringToHash("Aim_Amount");

    public CharacterController myController;

    private Vector3 currentMovement;

    private Quaternion inputRotation;

    //private float gravityStrength = 15f;

    public override void Start()
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
            curSkill.Character = this;
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

        for (int i = 0; i < ActiveSkills.Length; i++)
        {
            ActiveSkills[i].LateSkillSetup();
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

        if (canWalk)
        {
            if (moveVector.magnitude > Deadzone)
            {
                isWalking = true;
                moveVector = inputRotation * moveVector;

                currentMovement += moveVector * Acceleration;
                float speed = currentMovement.magnitude;
                if (speed > (MoveSpeed * MoveSpeedMultiplicator))
                {
                    currentMovement *= (MoveSpeed * MoveSpeedMultiplicator) / speed;
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
            if ((lookVector.magnitude > Deadzone))
            {
                isUsingRightStick = true;

                lookVector = inputRotation * lookVector;

                Anim.SetBool(animIsAiming, true);
                Anim.SetBool(animIsRunning, false);
                Anim.SetFloat(animIsAim_Amount, lookVector.magnitude);
                Anim.SetFloat(animMovY, Mathf.Clamp(moveVector.y, -1, 0.5f));
                Anim.SetFloat(animMovX, moveVector.x);
            }
            else
            {
                isUsingRightStick = false;

                Anim.SetBool(animIsRunning, true);
                Anim.SetFloat(animMovX, moveVector.magnitude);
            }
        }
        else
        {
            Anim.SetFloat(animMovX, moveVector.magnitude);
        }

        if (isUsingRightStick)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookVector), 0.35f);
        }
        else if (isWalking)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveVector), TurnSpeed);
        }

        if (isWalking)
        {
            Walk(currentMovement);
            //myController.Move(currentMovement * Time.deltaTime);
        }

    }

    public void Walk(Vector3 currentMovementArg)
    {
        CollisionFlags flags = myController.Move(currentMovementArg * Time.deltaTime);
    }

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
                        if (Input.GetButton(thisPlayerString[usedButtons[tempIJ]]))
                        {
                            ActiveSkills[i].Shoot();
                            ActiveSkills[i].isFiring = true;
                            tempIsShooting = true;
                            //Debug.Log("Fire " + this.gameObject.name + " " + PlayerNumber + " Joystick " + Input.GetJoystickNames() + " ShootButton" + " isButton" + areButtons[i] + " PlayerString" + thisPlayerString[usedButtons[i]]);
                        }
                        else if ((Input.GetButtonUp(thisPlayerString[usedButtons[tempIJ]])) && PlayerSkills[i].ButtonStringBC.Length == 1)
                        {
                            ActiveSkills[i].isFiring = false;
                            ActiveSkills[i].StopShoot();
                            //Debug.Log("StopFire " + this.gameObject.name + " " + PlayerNumber + " Joystick " + Input.GetJoystickNames() + " ShootTriggern" + " isButton" + areButtons[i] + " PlayerString" + thisPlayerString[usedButtons[i]]);
                            tempIsShooting = true;
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
                }
            }
        }
    }

    public override void Update()
    {
        Move();
        CheckButtonInput();
        base.Update();
    }
}
