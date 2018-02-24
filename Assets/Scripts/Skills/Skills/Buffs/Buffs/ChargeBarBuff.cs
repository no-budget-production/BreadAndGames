using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBarBuff : Buff
{
    public Slider ChargeBar;
    public float ChargeBarMax = 100;
    public float curChargeBar;
    public float ChargeBarStep;

    public PlayerController ReviveTarget;

    private void Start()
    {
        ChargeBarStep = Lifetime;
        Fade();
        ChargeBar.maxValue = Lifetime;
        StartCoroutine(Fade());
    }

    public void OnActionBarChange()
    {
        ChargeBar.value = curChargeBar;
    }

    public override IEnumerator Fade()
    {
        float duration = Time.time + Lifetime;

        var tempRevive = Skill.GetComponent<Revive>();

        bool failed = false;

        while ((Time.time < duration) && !failed)
        {
            if (ReviveTarget.CurrentHealth > 0)
            {
                failed = true;
            }

            if (Vector3.Distance(Character.transform.position, ReviveTarget.transform.position) > tempRevive.ReviveRange)
            {
                failed = true;
            }

            curChargeBar += Time.deltaTime;
            OnActionBarChange();
            yield return null;
        }

        if (tempRevive != null)
        {
            if (!failed)
            {
                tempRevive.OnComplete();
            }

        }

        Character.canWalk = true;

        Character.canUseRightStick = true;

        Character.canUseSkills = true;


        Character.ActiveBuffs.Remove(this);

        Destroy(gameObject);
    }
}
