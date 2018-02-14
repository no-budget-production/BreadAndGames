﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newHookEventListener : MonoBehaviour
{
    public delegate void OnHookTakeBackEvent();

    public event OnHookTakeBackEvent StartTakeBackHook;

    public void NotifyTakeBack()
    {
        // hook should take back
        StartTakeBackHook();
    }


    public delegate void OnHookSomethingEvent(Vector3 hookContactPoint);

    public event OnHookSomethingEvent HookSomething;

    public void NotifyHookSomething(Vector3 hookContactPoint)
    {
        // hook hooked something
        HookSomething(hookContactPoint);
    }

}

