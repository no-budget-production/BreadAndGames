using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeShieldActive : Skill
{
    private bool active = false;

    public override void Shoot()
    {
        var handler = Character.GetComponentInChildren<MeleeShieldHandler>();
        if (handler == null)
            return;
        handler.Active();
    }
}
