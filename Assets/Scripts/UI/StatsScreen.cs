using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatsScreen : MonoBehaviour
{
    public Text[] Stats;

    public int CreditsBuildIndex;

    Scene currentScene;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();

        CreditsBuildIndex = SceneManager.sceneCountInBuildSettings - 1;

        UpdateStats();
    }

    public void UpDateTime()
    {
        if (!(CreditsBuildIndex == currentScene.buildIndex))
        {
            StatsTracker.Instance.Time = Mathf.Round(Time.timeSinceLevelLoad * 10.0f) / 10.0f;
        }
    }

    public void UpdateStats()
    {
        UpDateTime();
        Stats[0].text = null;
        Stats[0].text += "Game Over: " + StatsTracker.Instance.GameOvers + " - ";
        Stats[0].text += "Wins: " + StatsTracker.Instance.Wins + " - ";



        Stats[0].text += "Time: " + TimeConversion(StatsTracker.Instance.Time) + " - ";

        Stats[0].text += "Session Best Time: " + TimeConversion(StatsTracker.Instance.SessionBestTime);
        //Stats[0].text += "Best Time: " + TimeConversion(StatsTracker.Instance.BestTime)+ "\n\n";

        int PlayerNumber = 1;

        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            Stats[PlayerNumber].text = "Player " + PlayerNumber + "\n\n";
            Stats[PlayerNumber].text += "Kills: " + StatsTracker.Instance.Kills[i] + "\n";
            Stats[PlayerNumber].text += "Damage Dealt: " + StatsTracker.Instance.DamageDealt[i] + "\n\n";

            Stats[PlayerNumber].text += "Downed: " + StatsTracker.Instance.Downed[i] + "\n";
            Stats[PlayerNumber].text += "Revived Teammate: " + StatsTracker.Instance.RevivedTeamMate[i] + "\n";
            Stats[PlayerNumber].text += "Revived Self: " + StatsTracker.Instance.RevivedSelf[i] + "\n\n";

            Stats[PlayerNumber].text += "HealthPacks: " + StatsTracker.Instance.HealthPacks[i] + "\n";
            Stats[PlayerNumber].text += "Healed: " + StatsTracker.Instance.Healed[i] + "\n";

            PlayerNumber++;
        }
    }

    string TimeConversion(float sec)
    {

        float minutes = Mathf.Floor(sec / 60);
        float seconds = sec % 60;

        string temp = string.Format("{0:0}:{1:00}", minutes, seconds);

        return temp;
    }
}
