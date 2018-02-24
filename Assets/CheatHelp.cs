using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CheatHelp : MonoBehaviour
{
    public Text[] Cheats;
    public string CheatsName = "Cheats";
    public Cheats Cheat;
    public List<CheatConfig> CheatsArray;
    public string[] PlayerNames;
    public PlayerController PlayerController;
    public List<ButtonConfig> ButtonConfigArray;
    //public List<CheatConfig> SortedList;



    void Start()
    {
        UpdateCheatButtons();
    }

    public void UpdateCheatButtons()
    {
        Cheat = GameObject.Find(CheatsName).GetComponent<Cheats>();

        CheatsArray = new List<CheatConfig>(Cheat.UsedCheats.Length);

        for (int i = 0; i < Cheat.UsedCheats.Length; i++)
        {
            CheatsArray.Add(Cheat.UsedCheats[i]);
        }

        for (int i = 0; i < CheatsArray.Count; i++)
        {
            Cheats[0].text += CheatsArray[i].ButtonStringBC.name + ": " + CheatsArray[i].Cheat.GetComponent<Cheat>().name + "\n";
        }
    }

    public void UpdatePlayerButtons()
    {
        PlayerNames = new string[GameManager.Instance.Players.Count];

        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            PlayerNames[i] = GameManager.Instance.Players[i].name;
        }

        int PlayerNumber = 1;

        for (int i = 0; i < PlayerNames.Length; i++)
        {
            PlayerController = GameObject.Find(PlayerNames[i]).GetComponent<PlayerController>();

            ButtonConfigArray = new List<ButtonConfig>(PlayerController.PlayerSkills.Length);

            for (int j = 0; j < PlayerController.PlayerSkills.Length; j++)
            {
                ButtonConfigArray.Add(PlayerController.PlayerSkills[j]);
            }

            for (int k = 0; k < ButtonConfigArray.Count; k++)
            {
                string tempString = null;

                for (int h = 0; h < ButtonConfigArray[k].ButtonStringBC.Length; h++)
                {
                    tempString += ButtonConfigArray[k].ButtonStringBC[h].name + ", ";
                }
                Cheats[PlayerNumber].text += tempString + ": " + ButtonConfigArray[k].SkillBC.GetComponent<Skill>().name + "\n";
            }

            PlayerNumber++;
        }
    }
}
