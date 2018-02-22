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

    public float HealAmount = 2.0f;
    public float MaxRange;

    private float healProcentage;

    Vector3 direction;

    bool isHalfWidthLineRenderer;

    bool isDroneSkillFound;

    public override void LateSkillSetup()
    {
        DroneOrigin = BeamOrigin;
        CurrentTarget = GameManager.Instance.GetPlayerByType(TargetType);
        CurrentTargetTransform = CurrentTarget.TakeHitPoint;
        healProcentage = CurrentTarget.MaxHealth * 0.01f * HealAmount;
    }

    public void FindDrone()
    {
        int atI = 0;

        for (int i = 0; i < Character.ActiveSkills.Length; i++)
        {
            if (Character.ActiveSkills[i].SkillType == SkillRequired.SkillType)
            {
                isDroneSkillFound = true;
                atI = i;
                break;
            }
        }

        if (isDroneSkillFound)
        {
            var temp = Character.ActiveSkills[atI].GetComponent<HealingDrone>();

            if (temp == null)
            {
                return;
            }
            DroneOrigin = temp.BeamOrigin.transform;
        }
    }

    public override void Shoot()
    {
        if (!isDroneSkillFound)
        {
            FindDrone();
        }

        if (!BuffObject.isStackable)
        {
            if (BuffObject.HasBuff(Character.ActiveBuffObjects))
            {
                return;
            }
        }

        if (BuffObject.HasCanTriggerWith(Character.ActiveBuffObjects))
        {
            return;
        }

        Character.AddBuff(BuffObject, 1, Character);

        LineRenderer.enabled = false;

        if (!InRange())
        {
            return;
        }

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
    }

    private void FixedUpdate()
    {
        if (!isFiring)
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

    bool InRange()
    {
        if (Vector3.Distance(CurrentTargetTransform.position, DroneOrigin.position) > MaxRange)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void OnHealObject(RaycastHit hit)
    {
        Entity healableObject = hit.collider.GetComponent<Entity>();
        if (healableObject != null)
        {
            if (BuffObject.HasEffectWith(Character.ActiveBuffObjects))
            {
                healableObject.GetHealth(healProcentage * 0.5f * Time.deltaTime);

                if (!isHalfWidthLineRenderer)
                {
                    LineRenderer.widthMultiplier = 0.5f;
                    isHalfWidthLineRenderer = true;
                }
            }
            else
            {
                healableObject.GetHealth(healProcentage * Time.deltaTime);

                if (isHalfWidthLineRenderer)
                {
                    LineRenderer.widthMultiplier = 1f;
                    isHalfWidthLineRenderer = false;
                }
            }
        }
    }
}


