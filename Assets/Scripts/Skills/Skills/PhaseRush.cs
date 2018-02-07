using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseRush : Skill
{
    public int Charges;
    public float ButtonCD;

    public bool isPhaseRushing;
    public bool isRechargingPhaseRushing;
    public float Duration;
    public float RechargeTime;
    public float MoveSpeedBonus;

    float nextShotTime;

    public int Dummy;

    private Coroutine coroutineDuration;
    private Coroutine coroutineRechargeTimer;

    public override void Shoot()
    {
        if (!isPhaseRushing)
        {
            if (Charges > 0)
            {
                if (Time.time > nextShotTime)
                {
                    isPhaseRushing = true;
                    nextShotTime = Time.time + ButtonCD;

                    base.PlayerController.moveSpeed += MoveSpeedBonus;
                    Player.layer = LayerMask.NameToLayer("Ignore Raycast");
                    Charges--;
                    if (!isRechargingPhaseRushing)
                    {
                        isRechargingPhaseRushing = true;
                        coroutineDuration = StartCoroutine(DurationTimer(Duration));
                    }
                }
            }
        }
    }

    IEnumerator DurationTimer(float duration)
    {
        //Debug.Log("PhaseRush_On");

        yield return new WaitForSeconds(duration);

        base.PlayerController.moveSpeed -= MoveSpeedBonus;
        Player.layer = LayerMask.NameToLayer("Default");
        isPhaseRushing = false;
        if (!isRechargingPhaseRushing)
        {
            coroutineDuration = StartCoroutine(RechargeTimer(RechargeTime));
        }
    }

    IEnumerator RechargeTimer(float rechargeTime)
    {
        Debug.Log("PhaseRush_Off");

        yield return new WaitForSeconds(rechargeTime);

        Charges++;
        if (Charges > 3)
        {
            coroutineDuration = StartCoroutine(DurationTimer(Duration));
        }
        else
        {
            isRechargingPhaseRushing = false;
        }
    }
}
