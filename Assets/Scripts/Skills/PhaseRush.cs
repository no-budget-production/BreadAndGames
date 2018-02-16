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
    //public float MoveSpeedBonus;
    public float Distance;

    public Effect thunder;
    public Effect curThunder;

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
                    SkillEvents();

                }
            }
        }
    }

    public void SpawnEffect()
    {
        curThunder = Instantiate(thunder, base.PlayerController.transform.position, base.PlayerController.transform.rotation);
        curThunder.GetComponent<ParticleSystem>().Play();
    }

    private void SkillEvents()
    {
        isPhaseRushing = true;
        Charges--;

        nextShotTime = Time.time + ButtonCD;

        SpawnEffect();

        base.SpawnBuff();

        StopCoroutine("DurationTimer");
        StartCoroutine(DurationTimer());

        base.PlayerController.canWalk = false;

        //base.PlayerController.moveSpeed += MoveSpeedBonus;

        base.PlayerController.ThisUnityTypeFlags = UnitTypesFlags.Invurnable;
        base.PlayerController.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    public IEnumerator DurationTimer()
    {
        base.PlayerController.transform.Translate(Vector3.forward * Distance);

        //float curTime = Time.time + Duration;

        //while (Time.time < Duration)
        //{
        //    curTime += Time.deltaTime * 0.01f;
        //    base.PlayerController.transform.Translate(Vector3.forward * Distance);
        //    yield return null;
        //}

        yield return new WaitForSeconds(Duration);

        //base.PlayerController.moveSpeed -= MoveSpeedBonus;
        base.PlayerController.ThisUnityTypeFlags = UnitTypesFlags.Player;
        base.PlayerController.gameObject.layer = LayerMask.NameToLayer("Default");

        isPhaseRushing = false;

        //Debug.Log("PhaseRush_Off");

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
