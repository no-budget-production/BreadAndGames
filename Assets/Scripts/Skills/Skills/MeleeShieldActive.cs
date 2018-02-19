using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeShieldActive : Skill
{
    private bool active = false;

    public override void Shoot()
    {
        if (active) return;
        active = true;
        var handler = Character.GetComponentInChildren<MeleeShieldHandler>();
        if (handler == null) return;
        handler.Active();
    }
    public override void StopShoot()
    {
        active = false;
    }
}
