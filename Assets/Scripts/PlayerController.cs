using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public CharacterController myController;
    public Collider PlayerTrigger;
    public float moveSpeed = 1.0f;
    public float jumpSpeed = 20.0f;
    public float maxJumpSpeed = 20.0f;
    public float gravityStrength = 15f;
    public float increaseJumpSpeed = 10f;
    private float jumpTimer;
    private float jumpTimerCheckRate = 1.0f;
    private float angle;

    //ParticleSystem
    public ParticleSystem dust;
    public bool includeChildren = true;

    public float airTime = -2f;

    float VerticalVelocity;

    public bool canJump = true;

    public Transform playerCamera;
    private Transform CurrentTransform;

    Vector3 currentMovement;

    public float acceleration = 1.2f;

    public Vector3 temporaryVector;
    public Vector3 temporarylookVector;

    void Start()
    {
        PlayerTrigger = GetComponent<Collider>();
        CurrentTransform = playerCamera;


        dust = GetComponent<ParticleSystem>();

    }

    void Update()
    {
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

        lookVector.x += Input.GetAxis("HorizontalLook1");
        temporarylookVector.x = Input.GetAxis("HorizontalLook1");
        lookVector.z += Input.GetAxis("VerticalLook1");
        temporarylookVector.z = Input.GetAxis("VerticalLook1");
        //lookVector = Vector3.ClampMagnitude(lookVector, 3.0f);
        angle = Mathf.Atan2(lookVector.x, lookVector.z) * Mathf.Rad2Deg;



        //moves the character
        CollisionFlags flags = myController.Move(myVector);

        if (myVector.x != 0 || myVector.z != 0)
        {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(myVector), 0.5f);
        } 
        if(lookVector.x != 0 || lookVector.z != 0)
        {
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }
}
