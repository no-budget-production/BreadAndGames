using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBarBuff : Buff
{
    public Slider ChargeBar;
    public float ChargeBarMax = 100;
    public float curChargeBar;
    public float ChargeBarStep;

    public PlayerController ReviveTarget;

    public Transform[] PLayerControllerTransforms;

    private void Start()
    {
        ChargeBarStep = Lifetime2;
        Fade();
        ChargeBar.maxValue = Lifetime2;
        StartCoroutine(Fade());

        PLayerControllerTransforms = new Transform[2];

        PLayerControllerTransforms[0] = Character.GetTransform();
        PLayerControllerTransforms[1] = ReviveTarget.GetTransform();
    }

    public void OnActionBarChange()
    {
        ChargeBar.value = curChargeBar;
    }

    public override IEnumerator Fade()
    {
        float duration = Time.time + Lifetime2;

        var tempRevive = Skill.GetComponent<Revive>();

        bool failed = false;

        while ((Time.time < duration) && !failed)
        {
            if (ReviveTarget.CurrentHealth > 0)
            {
                failed = true;
            }

            if (Vector3.Distance(PLayerControllerTransforms[0].position, PLayerControllerTransforms[1].position) > tempRevive.ReviveRange)
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

        if (!Character.canNeverUseRightStick)
        {
            Character.canUseRightStick = true;
        }

        Character.canUseSkills = true;


        Character.ActiveBuffs.Remove(this);

        Destroy(gameObject);
    }
}
