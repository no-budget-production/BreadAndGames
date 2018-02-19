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
        curThunder = Instantiate(thunder, base.Character.transform.position, base.Character.transform.rotation);
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

        base.Character.canWalk = false;

        //base.PlayerController.moveSpeed += MoveSpeedBonus;

        base.Character.ThisUnityTypeFlags = UnitTypesFlags.Invurnable;
        base.Character.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    public IEnumerator DurationTimer()
    {
        base.Character.transform.Translate(Vector3.forward * Distance);

        //float curTime = Time.time + Duration;

        //while (Time.time < Duration)
        //{
        //    curTime += Time.deltaTime * 0.01f;
        //    base.PlayerController.transform.Translate(Vector3.forward * Distance);
        //    yield return null;
        //}

        yield return new WaitForSeconds(Duration);

        //base.PlayerController.moveSpeed -= MoveSpeedBonus;
        base.Character.ThisUnityTypeFlags = UnitTypesFlags.Player;
        base.Character.gameObject.layer = LayerMask.NameToLayer("Default");

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
