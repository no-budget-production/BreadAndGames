using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBuff : Buff
{

    private void Start()
    {
        base.StartCoroutine(Fade());
    }

    private void OnDestroy()
    {
        Debug.Log("Buff Destroyed");
    }
}
