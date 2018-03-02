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
    public int InternalPlayerNumber;
    public string PlayerNumber;

    public ButtonConfig[] PlayerSkills;
    public float Deadzone;

    public float acceleration;
    public float deceleration;
    private float DecelerationHelp;
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
    public bool isWalking;
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

    public bool RequestedHealthPickUps;

    public GameObject Canvas;

    public Image DamageImage;
    public float FlashSpeed = 1f;
    public Color DamageFlashColour = new Color(1f, 0f, 0f, 0.1f);
    public Color ReviveFlashColour = new Color(1f, 1f, 1f, 0.2f);

    public bool wasDead;
    public bool wasRevived;

    //public PlayerStats PlayerStats;

    void Awake()
    {
        _Animtor.SetBool(animIsAiming, true);
        _Animtor.SetBool(animIsRunning, false);

        rb = GetComponent<Rigidbody>();
    }

    public override void Start()
    {
        NPC = false;
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
        if (isDeadTrigger)
            return;
        Horizontal_PX = Input.GetAxis(thisPlayerString[0]);
        Vertical_PX = Input.GetAxis(thisPlayerString[1]);
        HorizontalLook_PX = Input.GetAxis(thisPlayerString[2]);
        VerticalLook_PX = Input.GetAxis(thisPlayerString[3]);
        Vector3 temporaryMoveVector = new Vector3(Horizontal_PX, 0, Vertical_PX);

        Vector3 temporaryLookVector = new Vector3(VerticalLook_PX, 0, HorizontalLook_PX);
        if (temporaryLookVector.magnitude > Deadzone)
        {
            temporaryLookVector = inputRotation * temporaryLookVector;
            lookVector = temporaryLookVector;
        }

        //Vector3 temporaryMoveVector = new Vector3(Horizontal_PX, 0, Vertical_PX);
        //if (temporaryMoveVector.magnitude > Deadzone)
        //{
        //    temporaryMoveVector = inputRotation * temporaryMoveVector;
        //    moveVector = temporaryMoveVector;
        //}

        if (canUseRightStick && !canNeverUseRightStick)
        {
            if (temporaryLookVector.magnitude > Deadzone)
            {
                isUsingRightStick = true;

                temporaryLookVector = inputRotation * temporaryLookVector;
                lookVector = temporaryLookVector;

                _Animtor.SetBool(animIsAiming, true);
                _Animtor.SetBool(animIsRunning, false);
                _Animtor.SetFloat(animIsAim_Amount, temporaryLookVector.magnitude);
                _Animtor.SetFloat(animMovY, Mathf.Clamp(temporaryMoveVector.y, -1, 0.5f));
                _Animtor.SetFloat(animMovX, temporaryMoveVector.magnitude);
            }
            else
            {
                isUsingRightStick = false;

                _Animtor.SetBool(animIsAiming, false);
                _Animtor.SetBool(animIsRunning, true);
                _Animtor.SetFloat(animMovX, temporaryMoveVector.magnitude);
            }
        }
        else
        {
            _Animtor.SetFloat(animMovX, temporaryMoveVector.magnitude);
        }

        Quaternion newLookDirection;
        if (isUsingRightStick && rotatable)
        {
            newLookDirection = Quaternion.Slerp(Quaternion.LookRotation(transform.forward, transform.up), Quaternion.LookRotation(lookVector, transform.up), TurnSpeed * Time.deltaTime);
            rb.rotation = newLookDirection;
        }

        if (canWalk)
        {
            if (temporaryMoveVector.magnitude > Deadzone)
            {
                isWalking = true;
                if (!isUsingRightStick && rotatable)
                {
                    newLookDirection = Quaternion.Slerp(Quaternion.LookRotation(transform.forward, transform.up), Quaternion.LookRotation(temporaryMoveVector, transform.up), TurnSpeed * Time.deltaTime);
                    rb.rotation = newLookDirection;
                }
                moveVector = temporaryMoveVector;
                Walk();
            }
            else
            {
                currentMovement = Vector3.zero;
                if (rb.velocity.magnitude > 0.2f)
                {
                    rb.AddForce(-rb.velocity * DecelerationHelp * Time.deltaTime, ForceMode.Acceleration);
                }
                else
                {
                    rb.velocity = Vector3.zero;
                    isWalking = false;
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
                        if (Input.GetButtonDown(thisPlayerString[usedButtons[tempIJ]]) && !ActiveSkills[i].WhileDead)
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
        else
        {
            int tempIJ = 0;
            for (int i = 0; i < PlayerSkills.Length; i++)
            {
                for (int j = 0; j < PlayerSkills[i].ButtonStringBC.Length; j++)
                {
                    if (areButtons[tempIJ])
                    {
                        if (Input.GetButtonDown(thisPlayerString[usedButtons[tempIJ]]) && ActiveSkills[i].WhileDead)
                        {
                            ActiveSkills[i].OneShoot();
                            ActiveSkills[i].isFiring = true;
                            //Debug.Log("Fire " + this.gameObject.name + " " + PlayerNumber + " Joystick " + Input.GetJoystickNames() + " ShootButton" + " isButton" + areButtons[i] + " PlayerString" + thisPlayerString[usedButtons[i]]);
                        }
                    }
                    tempIJ++;
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
        Grounded();
    }

    void Grounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 0.5f) == false)
        {
            DecelerationHelp = 0;
        }
        else
        {
            DecelerationHelp = deceleration;
        }
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
            int areBothPlayersDead = 0;
            for (int i = 0; i < GameManager.Instance.Players.Count; i++)
            {
                if (GameManager.Instance.Players[i].isDeadTrigger == true)
                {
                    areBothPlayersDead++;
                }
            }

            if (areBothPlayersDead == GameManager.Instance.Players.Count)
            {
                GameManager.Instance.UIScript.GameOverText.text = "Game Over";
                GameManager.Instance.UIScript.GameOver();
            }
            //else
            {
                if (Type == PlayerType.Melee)
                {
                    GameManager.Instance.ReviveWheel_Melee.Activate();
                    DisableHUD();
                }

                if (Type == PlayerType.Shooter)
                {
                    GameManager.Instance.ReviveWheel_Shooter.Activate();
                    DisableHUD();
                }

            }

            wasRevived = false;
            wasDead = true;

            DamageImage.color = DamageFlashColour;
            StopCoroutine(ColorFlash());
            StartCoroutine(ColorFlash());

            _Animtor.SetBool(animIsDead, true);

            StatsTracker.Instance.Downed[InternalPlayerNumber]++;
        }

        RequestHealthPickUps();
    }

    public override void GetHealth(float healing)
    {
        base.GetHealth(healing);

        if (!isDeadTrigger)
        {
            _Animtor.SetBool(animIsDead, false);
            _Animtor.SetTrigger(animGetUp);

            if (wasDead)
            {
                EnableHUD();
                wasRevived = true;
                wasDead = false;

                DamageImage.color = ReviveFlashColour;
                StopCoroutine(ColorFlash());
                StartCoroutine(ColorFlash());
            }
        }

        RequestHealthPickUps();
    }

    public void RequestHealthPickUps()
    {
        if (CurrentHealth / MaxHealth < GameManager.Instance.PickUpSpawner.HealthPickUpThreshold * 0.01)
        {
            if (!RequestedHealthPickUps)
            {
                GameManager.Instance.PickUpSpawner.HealthRequestAdding(true);
                RequestedHealthPickUps = true;
            }
        }
        else
        {
            if (RequestedHealthPickUps)
            {
                GameManager.Instance.PickUpSpawner.HealthRequestAdding(false);
                RequestedHealthPickUps = false;
            }
        }
    }

    public void DisableHUD()
    {
        Canvas.SetActive(false);
        UseHUDHealthbarSlider = false;
        UseHUDActionPointsBar = false;
        UseOverChargeBar = false;
    }

    public void EnableHUD()
    {
        Canvas.SetActive(true);
        UseHUDHealthbarSlider = true;
        UseHUDActionPointsBar = true;
        UseOverChargeBar = true;
    }



    public virtual IEnumerator ColorFlash()
    {
        while (DamageImage.color != Color.clear)
        {
            DamageImage.color = Color.Lerp(DamageImage.color, Color.clear, FlashSpeed * Time.deltaTime);
            yield return null;
        }
    }
}