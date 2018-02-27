﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;

public class ReviveSelf_Melee : Skill
{
    public int howMutchToGet;
    public float ReviveHealthMulti;
    public float workRadius;

    private ActivateWheel_Melee UI;
    private findIt[] block;
    private int youGetIt = 0;
    private int youGetItNot = 0;

    // Use this for initialization
    void Start()
    {
        WhileDead = true;
        UI = FindObjectOfType<ActivateWheel_Melee>();
    }

    // Update is called once per frame

    public override void OneShoot()
    {
        block = FindObjectsOfType<findIt>();
        var wheel = UI.GetComponentInChildren<ReviveWheelSpin>();
        foreach (findIt b in block)
        {
            var check = Mathf.Abs(b.GetComponent<RectTransform>().eulerAngles.z);
            if (check <= workRadius || check >= (360 - workRadius))
            {
                Debug.Log("check: " + check);
                youGetIt++;
                wheel.increaseSpeed();
                Destroy(b.gameObject);
                wheel.changeDirection();
            }
            else
            {
                youGetItNot++;
            }
        }
        block = FindObjectsOfType<findIt>();
        if (block.Length <= youGetItNot)
        {
            wheel.resetSpeed();
            youGetIt = 0;
            foreach (findIt b in block)
            {
                Destroy(b.gameObject);
            }
            UI.Deactivate();
            UI.Activate();
        }
        youGetItNot = 0;
        if (howMutchToGet <= youGetIt)
        {
            wheel.resetSpeed();
            Deactivate();
        }
    }

    public void Deactivate()
    {
        
        block = FindObjectsOfType<findIt>();
        foreach (findIt b in block)
        {
            Destroy(b.gameObject);
        }
        Character.GetHealth(Character.MaxHealth * ReviveHealthMulti);
        youGetIt = 0;
        UI.Deactivate();
    }
}
