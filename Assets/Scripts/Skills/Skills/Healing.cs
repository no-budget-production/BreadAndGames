using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : Skill
{
    public float energyCosts;

    public LineRenderer LineRenderer;
    public PlayerController CurrentTarget;
    public Transform CurrentTargetTransform;
    public Transform BeamOrigin;
    //public Transform DroneOrigin;
    public PlayerType TargetType;

    public BuffObject IsHealing;

    //public Skill SkillRequired;

    public float HealAmount = 2.0f;
    public float ActionPointAmount = 2.0f;
    public float ReloadAmount = 2.0f;

    public float MaxRange;

    private float healProcentage;
    private float actionProcentage;
    private float reloadProcentage;

    Vector3 direction;

    bool isHalfWidthLineRenderer;

    bool isDroneSkillFound;

    public override void LateSkillSetup()
    {
        BeamOrigin = SkillSpawn;
        //DroneOrigin = BeamOrigin;
        CurrentTarget = GameManager.Instance.GetPlayerByType(TargetType);
        CurrentTargetTransform = CurrentTarget.TakeHitPoint;
        healProcentage = CurrentTarget.MaxHealth * 0.01f * HealAmount;
        actionProcentage = CurrentTarget.maxActionPoints * 0.01f * ActionPointAmount;
        reloadProcentage = CurrentTarget.maxReloadBar * 0.01f * ReloadAmount;
    }

    //public void FindDrone()
    //{
    //    int atI = 0;

    //    for (int i = 0; i < Character.ActiveSkills.Length; i++)
    //    {
    //        if (Character.ActiveSkills[i].SkillType == SkillRequired.SkillType)
    //        {
    //            isDroneSkillFound = true;
    //            atI = i;
    //            break;
    //        }
    //    }

    //    if (isDroneSkillFound)
    //    {
    //        var temp = Character.ActiveSkills[atI].GetComponent<HealingDrone>();

    //        if (temp == null)
    //        {
    //            return;
    //        }
    //        DroneOrigin = temp.BeamOrigin.transform;
    //    }
    //}

    public override void Shoot()
    {
        //if (!isDroneSkillFound)
        //{
        //    FindDrone();
        //}
        if (Character.curActionPoints - energyCosts * Time.deltaTime < 0)
        {
            return;
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

        if (!((HealAmount != 0) && (CurrentTarget.CurrentHealth < CurrentTarget.MaxHealth) || ((ActionPointAmount != 0) && (CurrentTarget.curActionPoints < CurrentTarget.maxActionPoints) && CurrentTarget.rechargeActionBarDirectly) || ((ReloadAmount != 0) && (CurrentTarget.curReloadBar < CurrentTarget.maxReloadBar) && !CurrentTarget.rechargeActionBarDirectly)))
        {
            return;
        }


        if (!InRange())
        {
            return;
        }

        Character.AddBuff(BuffObject, 1, Character);

        LineRenderer.enabled = false;

        Character.curActionPoints -= energyCosts * Time.deltaTime;


        LockOn();

        LineRenderer.SetPosition(0, BeamOrigin.position);
        LineRenderer.SetPosition(1, CurrentTargetTransform.position);

        OnHealObject(CurrentTarget);

        LineRenderer.enabled = true;

        //RaycastHit hit;

        //if (Physics.Raycast(BeamOrigin.position, direction, out hit))
        //{
        //    var temp = hit.collider.GetComponent<PlayerController>();
        //    if (temp == CurrentTarget)
        //    {
        //        LineRenderer.SetPosition(0, BeamOrigin.position);
        //        LineRenderer.SetPosition(1, CurrentTargetTransform.position);

        //        OnHealObject(hit);

        //        LineRenderer.enabled = true;
        //    }
        //    else
        //    {
        //        LineRenderer.SetPosition(0, BeamOrigin.position);
        //        LineRenderer.SetPosition(1, transform.position);

        //        LineRenderer.enabled = false;
        //    }
    }


    private void FixedUpdate()
    {
        if (!isFiring)
        {
            LineRenderer.SetPosition(0, BeamOrigin.position);
            LineRenderer.SetPosition(1, transform.position);
            LineRenderer.enabled = false;
        }
    }

    void LockOn()
    {
        direction = CurrentTargetTransform.position - BeamOrigin.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(BeamOrigin.rotation, lookRotation, Time.deltaTime * 100000000).eulerAngles;
        BeamOrigin.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    bool InRange()
    {
        if (Vector3.Distance(CurrentTargetTransform.position, BeamOrigin.position) > MaxRange)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    //void OnHealObject(RaycastHit hit)
    void OnHealObject(PlayerController PlayerController)
    {
        //Entity healableObject = hit.collider.GetComponent<Entity>();
        //if (PlayerController != null)
        {
            //var tempCharacter = PlayerController.GetComponent<Character>();

            Character.AddBuff(IsHealing, 1, Character);

            if (IsHealing.HasEffectWith(Character.ActiveBuffObjects))
            {
                Debug.Log("HasEffectWith");

                //if (PlayerController != null)
                {
                    if (!PlayerController.rechargeActionBarDirectly)
                    {
                        PlayerController.RestoreReloadPoints(reloadProcentage * 0.5f * Time.deltaTime);
                    }
                    else
                    {
                        PlayerController.RestoreActionPoints(actionProcentage * 0.5f * Time.deltaTime);
                    }
                }

                PlayerController.GetHealth(healProcentage * 0.5f * Time.deltaTime);

                if (!isHalfWidthLineRenderer)
                {
                    LineRenderer.widthMultiplier = 0.5f;
                    isHalfWidthLineRenderer = true;
                }
            }
            else
            {
                Debug.Log("!HasEffectWith");

                //if (PlayerController != null)
                {
                    if (!PlayerController.rechargeActionBarDirectly)
                    {
                        PlayerController.RestoreReloadPoints(reloadProcentage * Time.deltaTime);
                    }
                    else
                    {
                        PlayerController.RestoreActionPoints(actionProcentage * Time.deltaTime);
                    }
                }

                PlayerController.GetHealth(healProcentage * Time.deltaTime);

                if (isHalfWidthLineRenderer)
                {
                    LineRenderer.widthMultiplier = 1f;
                    isHalfWidthLineRenderer = false;
                }
            }
        }
    }
}


