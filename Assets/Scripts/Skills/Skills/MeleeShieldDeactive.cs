using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeShieldDeactive : Skill {

    private bool active = false;

    public override void Shoot()
    {
        if (active) return;
        active = true;
        var handler = Character.GetComponentInChildren<MeleeShieldHandler>();
        if (handler == null) return;
        handler.Deactive();
    }
    public override void StopShoot()
    {
        active = false;
    }
}
