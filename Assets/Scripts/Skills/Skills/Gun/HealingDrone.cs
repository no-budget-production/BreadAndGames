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
    public PlayerController PlayerController;

    public Rigidbody controller;
    public DroneController DroneController;
    public Transform PlayerPostion;

    private float HorizontalLook_PX;
    private float VerticalLook_PX;

    public float acceleration = 1000.0F;
    public float deceleration = 1.0F;
    public float maxSpeed = 5;

    public Vector3 movement;

    public Transform BeamOrigin;

    public float Leeway;

    public Vector3 startMarker;
    public Vector3 endMarker;
    public float speed = 1.0F;
    private float startTime;
    private float journeyLength;
    private bool isReturning;

    public float Smoothing;
    public float SmoothingStep;
    public float SmoothingMin;
    public float SmoothinMax;

    void Start()
    {
        DroneController = Instantiate(DroneController, transform.position + DroneController.transform.position, Quaternion.identity);
        controller = DroneController.GetComponent<Rigidbody>();
        BeamOrigin = DroneController.BeamOrigin;

        var tempPlayerController = Character.GetComponent<PlayerController>();
        if (Character.GetComponent<PlayerController>() != null)
        {
            PlayerController = tempPlayerController;
        }

    }

    void Update()
    {
        HorizontalLook_PX = Input.GetAxis(PlayerController.thisPlayerString[2]);
        VerticalLook_PX = Input.GetAxis(PlayerController.thisPlayerString[3]);

        Vector3 lookVector = new Vector3(HorizontalLook_PX, 0, VerticalLook_PX);

        movement = new Vector3(VerticalLook_PX, 0.0f, HorizontalLook_PX).normalized;

        if (lookVector.magnitude < PlayerController.deadZones[0])
        {
            if (controller.velocity.magnitude > 0.2f)
                controller.AddForce(-controller.velocity * deceleration * Time.deltaTime);
            else
            {
                startMarker = controller.transform.position;
                endMarker = new Vector3(Character.transform.position.x, startMarker.y, Character.transform.position.z);
                Vector3 newSpot = Vector3.MoveTowards(startMarker, endMarker, Leeway);
                journeyLength = Vector3.Distance(startMarker, endMarker);
                if (true)
                {
                    if (!isReturning)
                    {
                        startTime = Time.time;
                    }


                    {
                        Smoothing += SmoothingStep;
                        Smoothing += Random.Range(SmoothingMin, SmoothinMax);

                        controller.transform.position = Vector3.Lerp(controller.transform.position, newSpot, Smoothing * Time.deltaTime);
                    }

                    isReturning = true;
                    //float distCovered = (Time.time - startTime) * speed;
                    //float fracJourney = distCovered / journeyLength;
                    //controller.transform.position = new Vector3(Mathf.SmoothStep(0, journeyLength, distCovered), startMarker.y, Mathf.SmoothStep(0, journeyLength, distCovered));
                }
                //else
                //{

                //    if (isReturning)
                //    {
                //        isReturning = false;
                //    }
                //    controller.velocity = Vector3.zero;
                //}
            }

        }
        else
        {
            controller.AddForce(movement * acceleration * Time.deltaTime);
        }
        controller.velocity = Vector3.ClampMagnitude(controller.velocity, maxSpeed);
        //Debug.Log(controller.velocity.magnitude);
    }

}
