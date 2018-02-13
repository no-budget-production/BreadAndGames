using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhasenSprintBuff : Buff
{

    private void Start()
    {
        base.StartCoroutine(Fade());
    }


}
