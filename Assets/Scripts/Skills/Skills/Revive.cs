using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revive : Skill
{
    public float ReviveRange;
    public float ReviveHealthMulti;
    public BuffObject ReviveCoolDownBuff;

    public override void Shoot()
    {
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
        Character.AddBuff(ReviveCoolDownBuff, 1, Character);

        var tempPlayer = Character.GetComponent<PlayerController>();
        if (tempPlayer == null)
        {
            return;
        }

        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            if (GameManager.Instance.Players[i] == tempPlayer)
            {
                continue;
            }

            if (!GameManager.Instance.Players[i].isDeadTrigger)
            {
                continue;
            }

            if (Vector3.Distance(Character.transform.position, GameManager.Instance.Players[i].transform.position) <= ReviveRange)
            {
                GameManager.Instance.Players[i].GetHealth(GameManager.Instance.Players[i].MaxHealth * ReviveHealthMulti);

                GameManager.Instance.Players[i].MaxHealth -= GameManager.Instance.Players[i].MaxHealth - (GameManager.Instance.Players[i].MaxHealth * ReviveHealthMulti);

                GameManager.Instance.Players[i].canWalk = true;

                GameManager.Instance.Players[i].canUseRightStick = true;

                GameManager.Instance.Players[i].canUseSkills = true;
            }
        }

    }

    public override void StopShoot()
    {

    }

}
