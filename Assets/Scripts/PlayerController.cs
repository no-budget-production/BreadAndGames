using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : InputManager
{
    [Range(0.0f, 1.0f)]
    public float TurnSpeed;

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




    void Update()
    {
        Vector3 moveVector = new Vector3(0, 0, 0);
        Vector3 lookVector = new Vector3(0, 0, 0);

        // XBox (left stick) movement input
        moveVector.x += Input.GetAxisRaw(XBoxHorizontalLeftStick) * acceleration;
        temporaryVector.x = Input.GetAxisRaw(XBoxHorizontalLeftStick);
        moveVector.z += Input.GetAxisRaw(XBoxVerticalLeftStick) * acceleration;
        temporaryVector.z = Input.GetAxisRaw(XBoxVerticalLeftStick);
        
        Quaternion inputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Camera.forward, Vector3.up)); // align movement to camera view (can put in start() if camera view doesnt change)

        moveVector = Vector3.ClampMagnitude(moveVector, 0.5f);
        moveVector = moveVector * moveSpeed * Time.deltaTime;
        moveVector = inputRotation * moveVector;

        // XBox (right stick) look input
        lookVector.z += Input.GetAxis(XBoxHorizontalRightStick);
        temporaryLookVector.x = Input.GetAxis(XBoxHorizontalRightStick);
        lookVector.x += Input.GetAxis(XBoxVerticalLookRightStick);
        temporaryLookVector.z = Input.GetAxis(XBoxVerticalLookRightStick);

        lookVector = inputRotation * lookVector;


        if (lookVector.magnitude < deadzone)
        {
            lookVector = Vector3.zero;
        }



        //moves the character
        CollisionFlags flags = myController.Move(moveVector);

        if (moveVector.x != 0 || moveVector.z != 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveVector), TurnSpeed);
            //transform.rotation = Quaternion.LookRotation(moveVector);
        }
        if (lookVector.x != 0 || lookVector.z != 0)
        {
            transform.rotation = Quaternion.LookRotation(lookVector);
        }

        //Weapon Input
        //Trigger
        if (Input.GetAxis(XBoxTriggerRight) < -0.25f)
        {
            Debug.Log("Trigger Right");
        }
        if (Input.GetAxis(XBoxTriggerLeft) < -0.25f)
        {
            Debug.Log("Trigger Left");
        }
        //Bumper
        if (Input.GetButtonDown(XBoxBumperLeft))
        {
            Debug.Log("Bumper Left");
        }
        if (Input.GetButtonDown(XBoxBumperRight))
        {
            Debug.Log("Bumper Right");
        }
        //Buttons
        if (Input.GetButtonDown(XBoxButtonA))
        {
            Debug.Log("Button A");
        }
        if (Input.GetButtonDown(XBoxButtonB))
        {
            Debug.Log("Button B");
        }
        if (Input.GetButtonDown(XBoxButtonX))
        {
            Debug.Log("Button X");
        }
        if (Input.GetButtonDown(XBoxButtonY))
        {
            Debug.Log("Button Y");
        }


        //else if (Input.GetButton("FirePC1"))
        //{
        //    gunSystem.Shoot();
        //}
    }
}
