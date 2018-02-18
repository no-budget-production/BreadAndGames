using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBuff : Buff
{

    float AccuracyBonus;

    private void Start()
    {
        base.StartCoroutine(Fade());
    }

    private void OnDestroy()
    {
        Debug.Log("Buff Destroyed");
    }
}
