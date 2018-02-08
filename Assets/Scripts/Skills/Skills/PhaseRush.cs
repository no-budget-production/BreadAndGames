using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseRush : Skill
{
    public int Charges;
    public int MaxCharges;
    public float ButtonCD;

    public bool isPhaseRushing;
    public bool isRechargingPhaseRushing;
    public float Duration;
    public float RechargeTime;
    public float MoveSpeedBonus;

    float nextShotTime;

    public int Dummy;

    bool b1, b2, b3, b4;

    //private Coroutine coroutineDuration;
    //private Coroutine coroutineRechargeTimer;

    public override void Shoot()
    {
        if (!isPhaseRushing)
        {
            if (Charges > 0)
            {
                if (Time.time > nextShotTime)
                {
                    if (true)
                    {
                        StopCoroutine("DurationTimer");
                        StartCoroutine(DurationTimer());
                    }
                }
            }
        }
    }

    public IEnumerator DurationTimer()
    {
        Debug.Log("PhaseRush_On");
        isPhaseRushing = true;
        nextShotTime = Time.time + ButtonCD;

        base.PlayerController.moveSpeed += MoveSpeedBonus;
        Player.layer = LayerMask.NameToLayer("Ignore Raycast");
        Charges--;

        yield return new WaitForSeconds(Duration);

        base.PlayerController.moveSpeed -= MoveSpeedBonus;
        Player.layer = LayerMask.NameToLayer("Default");
        isPhaseRushing = false;

        Debug.Log("PhaseRush_Off");

        while (Charges < MaxCharges)
        {
            isRechargingPhaseRushing = true;
            yield return new WaitForSeconds(RechargeTime);
            Charges++;
            Charges = Mathf.Min(Charges, MaxCharges);
        }

        isRechargingPhaseRushing = false;

    }
}
