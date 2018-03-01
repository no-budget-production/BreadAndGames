using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CheatConfig
{
    public Cheat Cheat;
    public ButtonString ButtonStringBC;
}

public class Cheats : MonoBehaviour
{
    public CheatConfig[] UsedCheats;
    public Cheat[] ActiveCheats;
    public int usedButtonsCount;
    public int[] usedButtons;
    public bool[] areButtons;
    public float[] deadZones;

    public string[] ButtonStrings = new string[]
    {
            "[0]",
            "[1]",
            "[2]",
            "[3]",
            "[4]",
            "[5]",
            "[6]",
            "[7]",
            "[8]",
            "[9]",
    };

    private void Awake()
    {
        ButtonSetup();
    }

    public void ButtonSetup()
    {
        usedButtonsCount = UsedCheats.Length;
        usedButtons = new int[usedButtonsCount];
        areButtons = new bool[usedButtonsCount];
        deadZones = new float[usedButtonsCount];
        ActiveCheats = new Cheat[usedButtonsCount];
        for (int i = 0; i < usedButtonsCount; i++)
        {
            usedButtons[i] = UsedCheats[i].ButtonStringBC.ButtonID;
            areButtons[i] = UsedCheats[i].ButtonStringBC.isButton;
            deadZones[i] = UsedCheats[i].ButtonStringBC.DeadZone;

            Cheat curSkill = Instantiate(UsedCheats[i].Cheat, transform.position, Quaternion.identity);
            curSkill.transform.SetParent(transform);
            ActiveCheats[i] = curSkill;
        }
    }

    private void CheckButtonInput()
    {
        for (int i = 0; i < usedButtonsCount; i++)
        {
            if (areButtons[i])
            {
                if (Input.GetButtonDown(ButtonStrings[usedButtons[i]]))
                {
                    ActiveCheats[i].Shoot();
                    ActiveCheats[i].isFiring = true;
                }
                else
                {
                    ActiveCheats[i].isFiring = false;
                }
            }
            else
            {
                if (Input.GetAxis(ButtonStrings[usedButtons[i]]) < deadZones[i])
                {
                    ActiveCheats[i].Shoot();
                    ActiveCheats[i].isFiring = true;
                }
                else
                {
                    ActiveCheats[i].isFiring = false;
                }
            }
        }
    }

    void Update()
    {
        CheckButtonInput();
    }
}
