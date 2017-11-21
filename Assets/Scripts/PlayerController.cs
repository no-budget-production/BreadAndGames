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

    //ParticleSystem
    public ParticleSystem dust;
    public bool includeChildren = true;

    public float airTime = -2f;

    float VerticalVelocity;

    public bool canJump = true;

    public Transform playerCamera;
    public Transform houseCamera;
    private Transform CurrentTransform;

    Vector3 currentMovement;

    AudioSource sound;

    public AudioClip run;
    public AudioClip jumpsnd;
    public AudioClip doublejumpsnd;
    public AudioClip jumpland;
    public AudioClip[] coinSound;

    private bool jumplandplayed;

    public bool candouble;

    public float acceleration = 1.2f;

    public Vector3 temporaryVector;

    void Start()
    {
        PlayerTrigger = GetComponent<Collider>();
        sound = GetComponent<AudioSource>();
        CurrentTransform = playerCamera;

        jumplandplayed = false;
        candouble = false;


        dust = GetComponent<ParticleSystem>();

    }

    void Update()
    {
        Vector3 myVector = new Vector3(0, 0, 0);

        if (canJump == true)
        {


            myVector.x += Input.GetAxis("HorizontalXbox1") * acceleration;
            temporaryVector.x = Input.GetAxis("HorizontalXbox1");
            myVector.z += Input.GetAxis("VerticalXbox1") * acceleration;
            temporaryVector.z = Input.GetAxis("VerticalXbox1");
            myVector = Vector3.ClampMagnitude(myVector, 3.0f);
            myVector = myVector * moveSpeed * Time.deltaTime;
            Quaternion inputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(CurrentTransform.forward, Vector3.up));
            myVector = inputRotation * myVector;

        }



        if (myVector.x != 0 || myVector.z != 0)
        {
            if (canJump)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(myVector), 0.3f);
            }
            else if (!canJump)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(myVector), 0.09f);
            }

            if (Input.GetButtonDown("Jump") && canJump)
            {

                VerticalVelocity += jumpSpeed;

                sound.PlayOneShot(jumpsnd);
                jumplandplayed = false;
                canJump = false;
                candouble = true;
                dust.Play(includeChildren);
            }


            VerticalVelocity -= gravityStrength * Time.deltaTime;
            myVector.y = VerticalVelocity * Time.deltaTime;

            //moves the character
            CollisionFlags flags = myController.Move(myVector);

            if ((flags & CollisionFlags.Below) != 0)
            {
                canJump = true;
                candouble = false;
                VerticalVelocity = -3f;
                jumpTimer = 0;

                if (!jumplandplayed)
                {
                    sound.PlayOneShot(jumpland);
                    jumplandplayed = true;
                }
            }
            else if ((flags & CollisionFlags.Sides) != 0)
            {
                canJump = false;
            }
            else
            {
                canJump = false;
                jumpTimer += Time.deltaTime;
            }


        }
    }
}
