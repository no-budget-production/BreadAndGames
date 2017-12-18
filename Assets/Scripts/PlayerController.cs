using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerController : Entity
{
    public CharacterController myController;
    public Collider PlayerTrigger;
    public Transform Camera;
    public Camera viewCamera;
    GunSystem gunSystem;

    public float moveSpeed = 1.0f;
    private float gravityStrength = 15f;

    private float angle;
    private float deadzone = 0.25f;

    Vector3 currentMovement;

    public float acceleration = 1.2f;

    public Vector3 temporaryVector;
    public Vector3 temporaryLookVector;


    protected override void Start()
    {
        base.Start();
        PlayerTrigger = GetComponent<Collider>();
        gunSystem = GetComponent<GunSystem>();
    }

    void Update()
    {
        Vector3 moveVector = new Vector3(0, 0, 0);
        Vector3 lookVector = new Vector3(0, 0, 0);

        //Movement Input Xbox
            moveVector.x += Input.GetAxis("HorizontalXbox1") * acceleration;
            temporaryVector.x = Input.GetAxis("HorizontalXbox1");
            moveVector.z += Input.GetAxis("VerticalXbox1") * acceleration;
            temporaryVector.z = Input.GetAxis("VerticalXbox1");


        //Movement Input PC
        moveVector.x += Input.GetAxis("HorizontalPC1") * acceleration;
        temporaryVector.x = Input.GetAxis("HorizontalPC1");
        moveVector.z += Input.GetAxis("VerticalPC1") * acceleration;
        temporaryVector.z = Input.GetAxis("VerticalPC1");

        moveVector = Vector3.ClampMagnitude(moveVector, 3.0f);
        moveVector = moveVector * moveSpeed * Time.deltaTime;
        Quaternion inputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Camera.forward, Vector3.up));
        moveVector = inputRotation * moveVector;
        


        //Look Input Xbox
        lookVector.z += Input.GetAxis("HorizontalLook1");
        temporaryLookVector.x = Input.GetAxis("HorizontalLook1");
        lookVector.x += Input.GetAxis("VerticalLook1");
        temporaryLookVector.z = Input.GetAxis("VerticalLook1");
        lookVector = inputRotation * lookVector;

        if (lookVector.magnitude < deadzone)
        {
            lookVector = Vector3.zero;
        }

       //Look Input Mouse **BUGGY**
       /*
       Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
       Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
       float rayDistance;
        
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 lookPoint = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, lookPoint, Color.red);
            Debug.DrawRay(ray.origin,ray.direction * 100,Color.red);
            Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
            transform.LookAt(heightCorrectedPoint);
        }*/

        //moves the character
        CollisionFlags flags = myController.Move(moveVector);

        if (moveVector.x != 0 || moveVector.z != 0)
        {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveVector), 0.5f);
        } 
        if(lookVector.x != 0 || lookVector.z != 0)
        {
            transform.rotation = Quaternion.LookRotation(lookVector);
        }

        //Weapon Input
        if (Input.GetAxis("FireXbox1") < -0.25f)
        {
            gunSystem.Shoot();

        }
        else if (Input.GetButton("FirePC1"))
            gunSystem.Shoot();

    }
}
