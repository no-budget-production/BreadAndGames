using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public int playerNumber;
    public bool XboxController = true;

    public Rigidbody playerRigidbody;
    public string movementAxisHorizontalName;
    public string movementAxisVerticalName;
    public string movementAxisHorizontalXboxName;
    public string movementAxisVerticalXboxName;

    public float movementInputValueHorizontal;
    public float movementInputValueVertical;
    public float movementSpeed;
    public Vector3 movement;

    private void FixedUpdate()
    {
        GetInput();
        Move();
    }

    public void Start()
    {
        SetInput();
    }

    public void SetInput()
    {
        movementAxisHorizontalName += playerNumber;
        movementAxisVerticalName += playerNumber;

        movementAxisHorizontalXboxName += playerNumber;
        movementAxisVerticalXboxName += playerNumber;
    }

    public void GetInput()
    {
        if (XboxController)
        {
            GetXboxInput();
        }
        else
        {
            GetKeyboardInput();
        }

    }

    public void GetXboxInput()
    {
        //movementInputValueHorizontal = Input.GetAxis(movementAxisHorizontalXboxName);
        //movementInputValueVertical = Input.GetAxis(movementAxisVerticalXboxName);

        float movementInputValueHorizontal = Input.GetAxis("HorizontalXbox1") * Time.deltaTime * movementSpeed;
        float movementInputValueVertical = Input.GetAxis("VerticalXbox1") * Time.deltaTime * movementSpeed;

        Vector3 moveDirection = new Vector3(Input.GetAxis("HorizontalXbox1"), + Input.GetAxis("VerticalXbox1"), 0);
        Vector3 moveAngle = new Vector3(0, +0, 45);
        transform.position += moveDirection * movementSpeed * Time.deltaTime;
        float angle = Mathf.Atan2(movementInputValueHorizontal, movementInputValueVertical) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));      
    }

    public void GetKeyboardInput()
    {
        movementInputValueHorizontal = Input.GetAxis(movementAxisHorizontalName);
        movementInputValueVertical = Input.GetAxis(movementAxisVerticalName);
    }

    public void Move()
    {
        movement.Set(movementInputValueHorizontal, 0.0f, movementInputValueVertical);

        movement = movement.normalized * movementSpeed * Time.deltaTime;

        playerRigidbody.MovePosition(transform.position + movement);
    }
}
