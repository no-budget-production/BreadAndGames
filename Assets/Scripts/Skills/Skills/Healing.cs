using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : Skill
{
    public LineRenderer LineRenderer;
    public Transform CurrentTarget;
    public Transform BeamOrigin;

    public float HealAmount = 2.0f;

    Vector3 direction;
    bool isVisible = false;

    bool isShooting;

    public string TargetTag;

    private void Start()
    {
        for (int i = 0; i < base.ActivePlayers.Length; i++)
        {
            if (base.ActivePlayers[i].tag == TargetTag)
            {
                CurrentTarget = base.ActivePlayers[i].transform;
                break;
            }
        }
    }

    public override void Shoot()
    {
        base.PlayerController.isInAction = true;

        LineRenderer.enabled = false;

        LockOn();

        RaycastHit hit;

        if (Physics.Raycast(BeamOrigin.position, direction, out hit))
        {
            if (hit.collider.tag == (TargetTag))
            {
                isShooting = true;

                isVisible = true;

                LineRenderer.SetPosition(0, BeamOrigin.position);
                LineRenderer.SetPosition(1, CurrentTarget.position);

                OnHealObject(hit);

                LineRenderer.enabled = true;
            }
            else
            {
                isShooting = false;

                isVisible = false;

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
        direction = CurrentTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(BeamOrigin.rotation, lookRotation, Time.deltaTime * 100000000).eulerAngles;
        BeamOrigin.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    //void RaycastCheck()
    //{


    //}

    void OnHealObject(RaycastHit hit)
    {
        Debug.Log(hit.collider.gameObject.name);
        Entity healableObject = hit.collider.GetComponent<Entity>();
        if (healableObject != null)
        {
            healableObject.GetHealth(HealAmount * 0.5f);
            Debug.Log("Fart");
        }
        else
        {
            healableObject.GetHealth(HealAmount);
            Debug.Log("AJKSNDLJH");
        }
    }
}


