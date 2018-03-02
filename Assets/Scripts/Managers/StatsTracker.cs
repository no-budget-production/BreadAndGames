using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class StatsTrackerHelper
{
    public static StatsTracker st(this MonoBehaviour s)
    {
        return StatsTracker.Instance;
    }
}

public class StatsTracker : MonoBehaviour
{
    public static StatsTracker Instance = null;

    public int GameOvers;
    public int Wins;

    public float Time;
    public float SessionBestTime;
    public float BestTime;

    public int[] Kills;
    public float[] DamageDealt;

    public int[] RevivedTeamMate;
    public int[] RevivedSelf;

    public int[] Downed;
    public int[] HealthPacks;
    public float[] Healed;

    void Awake()
    {
        InitGame();

        ResetStats();

        ResetSession();
    }

    void InitGame()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void ResetStats()
    {
        Time = 0;

        for (int i = 0; i < Kills.Length; i++)
        {
            Kills[i] = 0;
            DamageDealt[i] = 0;

            RevivedTeamMate[i] = 0;
            RevivedSelf[i] = 0;
            HealthPacks[i] = 0;
            Healed[i] = 0;
        }
    }

    public void ResetSession()
    {
        GameOvers = 0;
        Wins = 0;

        SessionBestTime = 0;
    }

    public void CalculateBestTimes()
    {
        if (SessionBestTime == 0)
        {
            SessionBestTime = Time;
        }

        if (BestTime == 0)
        {
            BestTime = SessionBestTime;
        }

        if (Time < SessionBestTime)
        {
            SessionBestTime = Time;

            if (SessionBestTime < BestTime)
            {
                BestTime = SessionBestTime;
            }
        }
    }
}