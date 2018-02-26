using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseRush : Skill
{
    public int maxCharges;
    public int curCharges = 3;

    private float nextSoundTime;
    public float SBetweenSounds;

    public float rechargeTime;
    private List<float> checkToNextChargeTest = new List<float>();

    public float duration;
    public float speed;

    public Transform thunder;
    private Transform curThunder;
    private PlayerController controller;
    private AudioSource _AudioSource;

    private bool isPhaseRushing;
    private bool isRecharging;

    void Awake()
    {
        _AudioSource = GetComponent<AudioSource>();
    }

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
        if (!controller.canUseSkills)
            return;
        if (curCharges <= 0)
            return;

        if (isPhaseRushing)
        {
            return;
        }

        curThunder = Instantiate(thunder, controller.transform.position, controller.transform.rotation).transform;
        curThunder.GetComponent<ParticleSystem>().Play();
        controller.canUseSkills = false;
        controller.rotatable = false;
        controller.moveable = false;

        _AudioSource.Play();

        Character.ThisUnityTypeFlags = UnitTypesFlags.Invurnable;
        Character.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        controller.rb.AddForce(controller.transform.forward * speed, ForceMode.Impulse);

        curCharges--;

        StopCoroutine("DurationTimer");
        StartCoroutine(DurationTimer());
    }

    public IEnumerator DurationTimer()
    {
        isPhaseRushing = true;

        yield return new WaitForSeconds(duration);

        isPhaseRushing = false;

        controller.rb.velocity = Vector3.zero;
        controller.canUseSkills = true;
        controller.rotatable = true;
        controller.moveable = true;

        Character.ThisUnityTypeFlags = UnitTypesFlags.Player;
        Character.gameObject.layer = LayerMask.NameToLayer("Default");

        if (!isRecharging)
        {
            isRecharging = true;
            while (curCharges < maxCharges)
            {
                yield return new WaitForSeconds(rechargeTime);
                curCharges++;
                curCharges = Mathf.Min(curCharges, maxCharges);
            }
            isRecharging = false;
        }

    }
}
