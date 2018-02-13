using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class HealingDrone : Skill
{
    public Rigidbody controller;
    public DroneController DroneController;


    private float HorizontalLook_PX;
    private float VerticalLook_PX;

    public float acceleration = 1000.0F;
    public float deceleration = 1.0F;
    public float maxSpeed = 5;

    public Vector3 movement;

    public Transform BeamOrigin;

    void Start()
    {
        DroneController = Instantiate(DroneController, transform.position + DroneController.transform.position, Quaternion.identity);
        controller = DroneController.GetComponent<Rigidbody>();
        BeamOrigin = DroneController.BeamOrigin;
    }

    public override void Shoot()
    {

    }

    void Update()
    {
        HorizontalLook_PX = Input.GetAxis(base.PlayerController.thisPlayerString[2]);
        VerticalLook_PX = Input.GetAxis(base.PlayerController.thisPlayerString[3]);

        movement = new Vector3(VerticalLook_PX, 0.0f, HorizontalLook_PX).normalized;

        if (HorizontalLook_PX == 0 && VerticalLook_PX == 0)
        {
            if (controller.velocity.magnitude > 0.2f)
                controller.AddForce(-controller.velocity * deceleration * Time.deltaTime);
            else
                controller.velocity = Vector3.zero;
        }
        else
        {
            controller.AddForce(movement * acceleration * Time.deltaTime);
        }
        controller.velocity = Vector3.ClampMagnitude(controller.velocity, maxSpeed);
        //Debug.Log(controller.velocity.magnitude);
    }

}
