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
}