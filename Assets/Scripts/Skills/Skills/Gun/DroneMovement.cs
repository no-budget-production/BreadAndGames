using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : Skill {

    public float movementSpeed;
    public CharacterController controller;
    public float value = 7.08f;
    private float Horizontal_PX;
    private float Vertical_PX;
    private float HorizontalLook_PX;
    private float VerticalLook_PX;
    private Vector3 moveDirection = Vector3.zero;
    public float speed = 6.0F;

    void Start()
    {
        controller = Instantiate(controller, transform.position + controller.transform.position, Quaternion.identity).GetComponent<CharacterController>();
    }

    public override void Shoot()
    {
        HorizontalLook_PX = Input.GetAxis(base.PlayerController.thisPlayerString[2]);
        VerticalLook_PX = Input.GetAxis(base.PlayerController.thisPlayerString[3]);
        Debug.Log("HorizontalLook_PX: " + HorizontalLook_PX);
        Debug.Log("VerticalLook_PX: " + VerticalLook_PX);
        moveDirection = new Vector3(HorizontalLook_PX, 0, VerticalLook_PX);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;

        controller.Move(moveDirection * Time.deltaTime);

        //controller.Move(new Vector3(HorizontalLook_PX, 1f, VerticalLook_PX));// * 0.09 * HorizontalLook_PX
       // controller.Move(controller.transform.right);// * 0.09 * HorizontalLook_PX
    }

}
