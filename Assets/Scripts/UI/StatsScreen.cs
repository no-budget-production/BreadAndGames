using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatsScreen : MonoBehaviour
{
    public Text[] Stats;
    public string[] PlayerNames;
    public PlayerController PlayerController;
    public List<ButtonConfig> ButtonConfigArray;

    void Start()
    {
        UpdateCheatButtons();
    }

    public void UpdateCheatButtons()
    {

        Stats[0].text += "Game Over: " + StatsTracker.Instance.GameOvers + " - ";
        Stats[0].text += "Wins: " + StatsTracker.Instance.Wins + " - ";
        Stats[0].text += "Time needed: " + StatsTracker.Instance.Time;
    }

    public void UpdatePlayerButtons()
    {


        PlayerNames = new string[GameManager.Instance.Players.Count];

        int PlayerNumber = 1;

        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            Stats[PlayerNumber].text += "Kills: " + StatsTracker.Instance.Kills[i] + "\n";
            Stats[PlayerNumber].text += "Damage Dealt: " + StatsTracker.Instance.DamageDealt[i] + "\n\n";
            Stats[PlayerNumber].text += "Revived Teammate: " + StatsTracker.Instance.RevivedTeamMate[i] + "\n";
            Stats[PlayerNumber].text += "Revived Self: " + StatsTracker.Instance.RevivedSelf[i] + "\n\n";
            Stats[PlayerNumber].text += "Downed: " + StatsTracker.Instance.Downed[i] + "\n";
            Stats[PlayerNumber].text += "HealthPacks: " + StatsTracker.Instance.HealthPacks[i] + "\n";
            Stats[PlayerNumber].text += "Healed: " + StatsTracker.Instance.Healed[i] + "\n";

            PlayerNumber++;
        }
    }
}
