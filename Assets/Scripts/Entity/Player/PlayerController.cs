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
    [SerializeField]
    public bool HasFlag(PlayerTypeFlags flags)
    {
        int typeflag = 1 << (int)Type;
        return (typeflag & (int)flags) != 0;
    }

    [Header(">>>>>>>>>> PlayerController:")]

    public PlayerType Type;
    public string PlayerNumber;

    public ButtonConfig[] PlayerSkills;
    public float Deadzone;

    public float acceleration;
    public float deceleration;
    public float moveSpeedMax;
    public float TurnSpeed;

    private float angle;
    private Quaternion inputRotation;

    private Vector3 currentMovement;

    private int animMovX = Animator.StringToHash("MovX");
    private int animMovY = Animator.StringToHash("MovY");
    private int animIsAiming = Animator.StringToHash("isAiming");
    private int animIsRunning = Animator.StringToHash("isRunning");
    private int animIsAim_Amount = Animator.StringToHash("Aim_Amount");
    private int animIsAttacking = Animator.StringToHash("isAttacking");
    private int animIsDead = Animator.StringToHash("isDead");
    private int animGetUp = Animator.StringToHash("GetUp");

    private int usedButtonsCount;
    private int[] usedButtons;
    private bool[] areButtons;
    private float[] deadZones;
    private string[] thisPlayerString;

    private bool isUsingRightStick;
    private bool isInAction;

    [HideInInspector]
    public bool rotatable = true;
    [HideInInspector]
    public bool moveable = true;

    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Vector3 moveVector;
    [HideInInspector]
    public Vector3 lookVector;

    private float Horizontal_PX;
    private float Vertical_PX;
    private float HorizontalLook_PX;
    private float VerticalLook_PX;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

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
            rb.rotation = newLookDirection;
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
                    rb.rotation = newLookDirection;
                }
                Walk();
            }
            else
            {
                currentMovement = Vector3.zero;
                if (rb.velocity.magnitude > 0.2f)
                {
                    rb.AddForce(-rb.velocity * deceleration * Time.deltaTime, ForceMode.Acceleration);
                }
                else
                {
                    rb.velocity = Vector3.zero;
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

    //void FixedUpdate()
    //{
    //    Move();
    //}

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
        rb.AddForce(moveVector * acceleration * Time.deltaTime, ForceMode.Acceleration);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, moveSpeedMax * MoveSpeedMultiplicator);
    }

    public override void TakeDamage(float damage, DamageType damageType)
    {
        base.TakeDamage(damage, damageType);

        if (isDeadTrigger)
        {
            Anim.SetTrigger(animIsDead);
        }
    }
}
