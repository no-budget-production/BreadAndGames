﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonString : MonoBehaviour
{
    public bool isButton;
    public int ButtonID;
    public float DeadZone;

    public string GetInputName(int player)
    {
        return name + player;
    }
}