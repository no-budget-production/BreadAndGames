using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Cheat
{

    bool isOn;

    public override void Shoot()
    {
        if (isOn)
        {
            isOn = false;
            GameObject.Find("Directional Light").transform.rotation = Quaternion.Euler(35.58f, 0, 0);
        }
        else
        {
            isOn = true;
            GameObject.Find("Directional Light").transform.rotation = Quaternion.Euler(-10, -25, -180);
        }
    }


}
