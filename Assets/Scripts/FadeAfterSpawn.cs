using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAfterSpawn : Effect
{
    void Awake()
    {
        Debug.Log("Start");
        StartCoroutine(Fade());
    }
}
