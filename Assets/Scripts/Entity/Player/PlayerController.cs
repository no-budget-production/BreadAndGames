using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



[System.Serializable]
public class ButtonConfig
{
    public Skill SkillBC;
    public ButtonString[] ButtonStringBC;
}

public class PlayerController : Character
{
    public PlayerType Type;

    [SerializeField]
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

    public bool isUsingRightStick;
    public bool isInAction;

    public ButtonConfig[] PlayerSkills;

    public float Deadzone;

    private float angle;

    int animMovX = Animator.StringToHash("MovX");
    int animMovY = Animator.StringToHash("MovY");
    int animIsAiming = Animator.StringToHash("isAiming");
    int animIsRunning = Animator.StringToHash("isRunning");
    int animIsAim_Amount = Animator.StringToHash("Aim_Amount");
    int animIsAttacking = Animator.StringToHash("isAttacking");
    int animSkill_0 = Animator.StringToHash("Skill_0");
    int animSkill_1 = Animator.StringToHash("Skill_1");
    //int animSkill_2 = Animator.StringToHash("Skill_2");
    int animIsDead = Animator.StringToHash("isDead");
    int animGetUp = Animator.StringToHash("GetUp");
    int animSkill_Bool_0 = Animator.StringToHash("Skill_Bool_0");

    private Quaternion inputRotation;

    public float Horizontal_PX;
    public float Vertical_PX;
    public float HorizontalLook_PX;
    public float VerticalLook_PX;

    //Movement
    public Vector3 moveVector;
    public Vector3 lookVector;

    public Rigidbody myController;

    private Vector3 currentMovement;
    public float acceleration;
    public float deceleration;
    public float moveSpeedMax;
    public float TurnSpeed;
    public bool rotatable = true;
    public bool moveable = true;

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
            curSkill.transform.position = SkillSpawn.position;
            curSkill.transform.rotation = SkillSpawn.rotation;
            curSkill.Character = this;
            curSkill.SkillSpawn = SkillSpawn;
            curSkill.SkillNumber = i;
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
        //Debug.Log(myController.velocity.magnitude);
        Horizontal_PX = Input.GetAxis(thisPlayerString[0]);
        Vertical_PX = Input.GetAxis(thisPlayerString[1]);
        HorizontalLook_PX = Input.GetAxis(thisPlayerString[2]);
        VerticalLook_PX = Input.GetAxis(thisPlayerString[3]);
        moveVector = new Vector3(Horizontal_PX, 0, Vertical_PX);

        Vector3 temporaryLookVector = new Vector3(VerticalLook_PX, 0, HorizontalLook_PX);
        //myController.angularVelocity = Vector3.zero;

        if (temporaryLookVector.magnitude > Deadzone)
        {
            temporaryLookVector = inputRotation * temporaryLookVector;
            lookVector = temporaryLookVector;
        }
        else
        {
            //lookVector = transform.forward;
        }

        if (canUseRightStick)
        {
            if (temporaryLookVector.magnitude > Deadzone)
            {
                isUsingRightStick = true;

                temporaryLookVector = inputRotation * temporaryLookVector;
                lookVector = temporaryLookVector;

                Anim.SetBool(animIsAiming, true);
                Anim.SetBool(animIsRunning, false);
                Anim.SetFloat(animIsAim_Amount, temporaryLookVector.magnitude);
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
            isUsingRightStick = false;
            Anim.SetFloat(animMovX, moveVector.magnitude);
        }

        if (isUsingRightStick && rotatable)
        {
            Quaternion newLookDirection = Quaternion.Slerp(Quaternion.LookRotation(transform.forward, transform.up), Quaternion.LookRotation(lookVector, transform.up), TurnSpeed * Time.deltaTime);
            myController.rotation = newLookDirection;
        }

        if (canWalk)
        {
            if (moveVector.magnitude > Deadzone)
            {
                //moveVector = inputRotation * moveVector;
                /*
                currentMovement += moveVector * accelerationBase;
                float speed = currentMovement.magnitude;
                if (speed > (MoveSpeed * MoveSpeedMultiplicator))
                {
                    currentMovement *= (MoveSpeed * MoveSpeedMultiplicator) / speed;
                }
                */
                if (!isUsingRightStick && rotatable)
                {
                    Quaternion newLookDirection = Quaternion.Slerp(Quaternion.LookRotation(transform.forward, transform.up), Quaternion.LookRotation(moveVector, transform.up), TurnSpeed * Time.deltaTime);
                    myController.rotation = newLookDirection;
                }
                Walk();
            }
            else
            {
                currentMovement = Vector3.zero;
                if (myController.velocity.magnitude > 0.2f)
                {
                    myController.AddForce(-myController.velocity * deceleration * Time.deltaTime);
                }
                else
                {
                    myController.velocity = Vector3.zero;
                }
            }
        }
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
                        if (Input.GetButtonDown(thisPlayerString[usedButtons[tempIJ]]))
                        {
                            ActiveSkills[i].OneShoot();
                            ActiveSkills[i].isFiring = true;
                            tempIsShooting = true;
                            //Debug.Log("Fire " + this.gameObject.name + " " + PlayerNumber + " Joystick " + Input.GetJoystickNames() + " ShootButton" + " isButton" + areButtons[i] + " PlayerString" + thisPlayerString[usedButtons[i]]);
                        }
                        else if (Input.GetButton(thisPlayerString[usedButtons[tempIJ]]))
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
        CheckButtonInput();
        base.Update();
        Move();
    }

    public void Walk()
    {
        if (!moveable)
            return;
        myController.AddForce(moveVector * acceleration * Time.deltaTime);
        myController.velocity = Vector3.ClampMagnitude(myController.velocity, moveSpeedMax);
    }
}
