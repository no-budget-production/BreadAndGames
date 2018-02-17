using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickCheck : Cheat
{
    public string Joysticks;

    public override void Shoot()
    {
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (Input.GetJoystickNames()[i] == null)
            {
                Joysticks += " " + i + ": null";
            }
            else
            {
                Joysticks += " " + i + ": " + Input.GetJoystickNames()[i].ToString();
            }
        }
        Debug.Log(Input.GetJoystickNames().Length + Joysticks);
        DebugConsole.Log(Input.GetJoystickNames().Length + Joysticks);
    }
}
