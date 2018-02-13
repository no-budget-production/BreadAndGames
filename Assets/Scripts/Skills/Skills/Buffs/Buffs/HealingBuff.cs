using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingBuff : Buff
{

    private void Start()
    {
        base.StartCoroutine(Fade());
    }


}
