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

        while (Time.time < duration)
        {
            curChargeBar += Time.deltaTime;
            OnActionBarChange();
            yield return null;
        }

        Character.ActiveBuffs.Remove(this);

        Destroy(gameObject);
    }
}
