﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : Skill
{
    public LineRenderer LineRenderer;
    public PlayerController CurrentTarget;
    public Transform BeamOrigin;
    public PlayerType TargetType;

    public float HealAmount = 2.0f;

    Vector3 direction;

    private void Start()
    {
        CurrentTarget = GameManager.Instance.GetPlayerByType(TargetType);
    }

    public override void Shoot()
    {
        base.PlayerController.isInAction = true;

        LineRenderer.enabled = false;

        LockOn();

        RaycastHit hit;

        if (Physics.Raycast(BeamOrigin.position, direction, out hit))
        {
            var temp = hit.collider.GetComponent<PlayerController>();
            if (temp == CurrentTarget)
            {
                LineRenderer.SetPosition(0, BeamOrigin.position);
                LineRenderer.SetPosition(1, CurrentTarget.transform.position);

                OnHealObject(hit);

                LineRenderer.enabled = true;
            }
            else
            {
                LineRenderer.SetPosition(0, BeamOrigin.position);
                LineRenderer.SetPosition(1, transform.position);

                LineRenderer.enabled = false;
            }
        }

        base.PlayerController.isInAction = false;
    }

    private void FixedUpdate()
    {
        if (!base.isFiring)
        {
            LineRenderer.SetPosition(0, BeamOrigin.position);
            LineRenderer.SetPosition(1, transform.position);
            LineRenderer.enabled = false;
        }
    }

    void LockOn()
    {
        direction = CurrentTarget.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(BeamOrigin.rotation, lookRotation, Time.deltaTime * 100000000).eulerAngles;
        BeamOrigin.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void OnHealObject(RaycastHit hit)
    {
        //Debug.Log(hit.collider.gameObject.name);
        Entity healableObject = hit.collider.GetComponent<Entity>();
        if (healableObject != null)
        {
            healableObject.GetHealth(HealAmount * 0.5f);
        }
        else
        {
            healableObject.GetHealth(HealAmount);
        }
    }
}


