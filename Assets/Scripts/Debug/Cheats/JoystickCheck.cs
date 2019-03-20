using UnityEngine;

public class JoystickCheck : Cheat
{
    public string Joysticks;

    public bool isOn;

    public FPSDisplay FPSDisplay;

    public FPSDisplay FPSDisplayInstance;

    public override void Shoot()
    {
        if (!isOn)
        {
            isOn = true;

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
            //Debug.Log(Input.GetJoystickNames().Length + Joysticks);
            DebugConsole.Log(Input.GetJoystickNames().Length + Joysticks);

            if (FPSDisplayInstance == null)
            {
                FPSDisplayInstance = Instantiate(FPSDisplay, Vector3.zero, Quaternion.identity);
                FPSDisplayInstance.transform.parent = gameObject.transform;
            }

            FPSDisplayInstance.enabled = true;
        }
        else
        {
            isOn = false;

            DebugConsole.Clear();

            if (FPSDisplayInstance != null)
            {
                FPSDisplayInstance.enabled = false;
            }
        }

    }
}
