using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseRush : Skill
{

    public int maxCharges;
    private int curCharges = 0;

    public float rechargeTime;
    private List<float> checkToNextChargeTest = new List<float>();

    public float duration;
    public float speed;

    public Transform thunder;
    private Transform curThunder;
    private PlayerController controller;

    public void Start()
    {
        var tempController = Character.GetComponent<PlayerController>();
        if (tempController != null)
        {
            controller = tempController;
        }
    }

    public override void OneShoot()
    {
        for (var i = 0; i < checkToNextChargeTest.Count; i++)
        {
            var timeStemp = checkToNextChargeTest[i];
            if (timeStemp <= Time.time && timeStemp != 0)
            {
                curCharges--;
                curCharges = Mathf.Max(curCharges, 0);
                checkToNextChargeTest.Remove(timeStemp);
            }
        }

        if (!controller.canUseSkills)
            return;
        if (curCharges >= maxCharges)
            return;

        curThunder = Instantiate(thunder, controller.transform.position, controller.transform.rotation).transform;
        curThunder.GetComponent<ParticleSystem>().Play();
        controller.canUseSkills = false;
        controller.rotatable = false;
        controller.moveable = false;

        Character.ThisUnityTypeFlags = UnitTypesFlags.Invurnable;
        Character.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        controller.rb.AddForce(controller.transform.forward * speed);

        if (curCharges == 0)
        {
            checkToNextChargeTest.Add(Time.time + rechargeTime);
        }
        else
        {
            checkToNextChargeTest.Add(checkToNextChargeTest[curCharges - 1] + rechargeTime);
        }
        curCharges++;
        Invoke("phaseRushStop", duration);
    }

    public void phaseRushStop()
    {
        Destroy(curThunder.gameObject);
        controller.rb.velocity = Vector3.zero;
        controller.canUseSkills = true;
        controller.rotatable = true;
        controller.moveable = true;

        Character.ThisUnityTypeFlags = UnitTypesFlags.Player;
        Character.gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
