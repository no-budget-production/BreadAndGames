using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Cheat
{
    public string DirectionalLight = "Directional Light";
    public Vector3 NightLight = new Vector3(-10, -25, -180);
    public Vector3 DayLight = new Vector3(-35.58f, 0, 0);
    public bool isLightOn;

    public override void Shoot()
    {
        if (isLightOn)
        {
            isLightOn = false;
            GameObject.Find("Directional Light").transform.rotation = Quaternion.Euler(35.58f, 0, 0);
        }
        else
        {
            isLightOn = true;
            GameObject.Find("Directional Light").transform.rotation = Quaternion.Euler(-10, -25, -180);
        }
    }
}