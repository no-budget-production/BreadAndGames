using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : Skill
{
    public LineRenderer LineRenderer;
    public PlayerController CurrentTarget;
    public Transform CurrentTargetTransform;
    public Transform BeamOrigin;
    public Transform DroneOrigin;
    public PlayerType TargetType;

    public Skill SkillRequired;
    public Buff BuffSynergy;

    public float HealAmount = 2.0f;
    private float healProcentage;

    Vector3 direction;

    bool isHalfWidthLineRenderer;

    private void Start()
    {
        DroneOrigin = BeamOrigin;
        CurrentTarget = GameManager.Instance.GetPlayerByType(TargetType);
        CurrentTargetTransform = CurrentTarget.TakeHitPoint;
        healProcentage = CurrentTarget.MaxHealth * 0.01f * HealAmount;
    }

    public void FindDrone()
    {
        int atI = 0;
        bool isDroneSkillFound = false;
        for (int i = 0; i < base.PlayerController.ActiveSkills.Length; i++)
        {
            if (base.PlayerController.ActiveSkills[i].SkillID == SkillRequired.SkillID)
            {
                //Debug.Log("SynergyFound: " + PlayerController.ActiveBuffs[i]);
                isDroneSkillFound = true;
                atI = i;
                break;
            }
        }

        if (isDroneSkillFound)
        {
            var temp = base.PlayerController.ActiveSkills[atI].GetComponent<HealingDrone>();

            if (temp == null)
            {
                Debug.Log("DoneMovementNotFound");
                return;
            }

            DroneOrigin = temp.BeamOrigin.transform;
        }
        else
        {
            Debug.Log("DroneSkillNotFound");
        }
    }

    public override void Shoot()
    {
        if (DroneOrigin = BeamOrigin)
        {
            FindDrone();
        }

        base.SpawnBuff();

        base.PlayerController.isInAction = true;

        LineRenderer.enabled = false;

        LockOn();

        RaycastHit hit;

        if (Physics.Raycast(DroneOrigin.position, direction, out hit))
        {
            var temp = hit.collider.GetComponent<PlayerController>();
            if (temp == CurrentTarget)
            {
                LineRenderer.SetPosition(0, DroneOrigin.position);
                LineRenderer.SetPosition(1, CurrentTargetTransform.position);

                OnHealObject(hit);

                LineRenderer.enabled = true;
            }
            else
            {
                LineRenderer.SetPosition(0, DroneOrigin.position);
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
            LineRenderer.SetPosition(0, DroneOrigin.position);
            LineRenderer.SetPosition(1, transform.position);
            LineRenderer.enabled = false;
        }
    }

    void LockOn()
    {
        direction = CurrentTargetTransform.position - DroneOrigin.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(DroneOrigin.rotation, lookRotation, Time.deltaTime * 100000000).eulerAngles;
        DroneOrigin.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    //bool firstHeal;
    //bool firstFullHeal;

    void OnHealObject(RaycastHit hit)
    {
        //Debug.Log(hit.collider.gameObject.name);
        Entity healableObject = hit.collider.GetComponent<Entity>();
        if (healableObject != null)
        {
            bool buffSynergyPresent = false;
            for (int i = 0; i < PlayerController.ActiveBuffs.Count; i++)
            {
                if (PlayerController.ActiveBuffs[i].BuffID == BuffSynergy.BuffID)
                {
                    //Debug.Log("SynergyFound: " + PlayerController.ActiveBuffs[i]);
                    buffSynergyPresent = true;
                    break;
                }
            }

            if (buffSynergyPresent)
            {
                Debug.Log("SynergyFound: " + base.UsedBuff.name);

                healableObject.GetHealth(healProcentage * 0.5f * Time.deltaTime);

                //if (!firstHeal)
                //{
                //    Debug.Log("RT:" + Time.realtimeSinceStartup);
                //    firstHeal = true;
                //}

                //if (healableObject.CurrentHealth == 100)
                //{
                //    if (!firstFullHeal)
                //    {
                //        Debug.Log("RT:" + Time.realtimeSinceStartup);
                //        Debug.Log(Time.realtimeSinceStartup);
                //        firstFullHeal = true;
                //    }

                //}

                if (!isHalfWidthLineRenderer)
                {
                    LineRenderer.widthMultiplier = 0.5f;
                    isHalfWidthLineRenderer = true;
                }


            }
            else
            {
                Debug.Log("SynergyNotFound: " + base.UsedBuff.name);
                healableObject.GetHealth(healProcentage * Time.deltaTime);

                //if (!firstHeal)
                //{
                //    Debug.Log(Time.realtimeSinceStartup);
                //    firstHeal = true;
                //}

                //if (healableObject.CurrentHealth == 100)
                //{
                //    if (!firstFullHeal)
                //    {
                //        Debug.Log("RT:" + Time.realtimeSinceStartup);
                //        Debug.Log(Time.realtimeSinceStartup);
                //        firstFullHeal = true;
                //    }

                //}

                if (isHalfWidthLineRenderer)
                {
                    LineRenderer.widthMultiplier = 1f;
                    isHalfWidthLineRenderer = false;
                }

            }


        }
        else
        {
            Debug.Log("DEADEND");
            //healableObject.GetHealth(HealAmount);
        }
    }
}


