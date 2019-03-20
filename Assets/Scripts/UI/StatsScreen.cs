using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatsScreen : MonoBehaviour
{
    public Text[] Stats;

    public int CreditsBuildIndex;

    Scene currentScene;

    private StatsTracker statsTracker;

    private void Awake()
    {
        statsTracker = StatsTracker.Instance;
    }

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
            statsTracker.Time = Mathf.Round(Time.timeSinceLevelLoad * 10.0f) / 10.0f;
        }
    }

    public void UpdateStats()
    {
        UpDateTime();
        Stats[0].text = null;
        Stats[0].text += "Game Over: " + statsTracker.GameOvers + " - ";
        Stats[0].text += "Wins: " + statsTracker.Wins + " - ";



        Stats[0].text += "Time: " + TimeConversion(statsTracker.Time) + " - ";

        Stats[0].text += "Session Best Time: " + TimeConversion(statsTracker.SessionBestTime);
        //Stats[0].text += "Best Time: " + TimeConversion(StatsTracker.Instance.BestTime)+ "\n\n";

        int PlayerNumber = 1;

        if (!statsTracker)
        {
            Awake();
        }


        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            Stats[PlayerNumber].text = "Player " + PlayerNumber + "\n\n";
            Stats[PlayerNumber].text += "Kills: " + statsTracker.Kills[i] + "\n";
            Stats[PlayerNumber].text += "Damage Dealt: " + statsTracker.DamageDealt[i] + "\n\n";

            Stats[PlayerNumber].text += "Downed: " + statsTracker.Downed[i] + "\n";
            Stats[PlayerNumber].text += "Revived Teammate: " + statsTracker.RevivedTeamMate[i] + "\n";
            Stats[PlayerNumber].text += "Revived Self: " + statsTracker.RevivedSelf[i] + "\n\n";

            Stats[PlayerNumber].text += "HealthPacks: " + statsTracker.HealthPacks[i] + "\n";
            Stats[PlayerNumber].text += "Healed: " + statsTracker.Healed[i] + "\n";

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
