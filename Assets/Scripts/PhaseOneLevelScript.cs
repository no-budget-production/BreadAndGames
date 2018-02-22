using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[SerializeField]
public enum WaveStatusReport
{
    off,
    begin,
    running,
    end, _length, _invalid = -1
}

//[ExecuteInEditMode]
public class PhaseOneLevelScript : MonoBehaviour
{
    //public int amountOfWaves;
    public bool startArenaEvent;

    private WaveStatusReport Wave1Status;
    private WaveStatusReport Wave2Status;
    private WaveStatusReport Wave3Status;
    private WaveStatusReport Wave4Status;

    public int PauseBetweenWaves;

    public int Wave1AmountOfEnemys;
    public int Wave2AmountOfEnemys;
    public int Wave3AmountOfEnemys;
    public int Wave4AmountOfEnemys;

    public ArenaSpawner[] spawner;

    //private string[] WaveNames;
    

    void Start()
    {
        startArenaEvent = false;
        Wave1Status = 0;
        Wave2Status = 0;
        Wave3Status = 0;
        Wave4Status = 0;
    }

    void Update ()
    {
        //if (!Application.isPlaying)
        //{
        //    WaveNames = new string[amountOfWaves];

        //    for (int i = 0; i < WaveNames.Length; i++)
        //    {
        //        WaveNames[i] = "Wave" + (i + 1);
        //    }
        //}
        if (startArenaEvent)
        {
            Wave1Status = WaveStatusReport.begin;
            startArenaEvent = false;
        }

        if (Application.isPlaying)
        {
            // Start of the Wave1
            if ((Wave1Status == WaveStatusReport.begin || Wave1Status == WaveStatusReport.running) && !(Wave1Status == WaveStatusReport.end))
            {
                StartWave(Wave1Status, Wave1AmountOfEnemys);
            }
            // Start of the Wave2
            if ((Wave2Status == WaveStatusReport.begin || Wave2Status == WaveStatusReport.running) && !(Wave2Status == WaveStatusReport.end))
            {
                StartWave(Wave2Status, Wave2AmountOfEnemys);
            }
            // Start of the Wave3
            if ((Wave3Status == WaveStatusReport.begin || Wave3Status == WaveStatusReport.running) && !(Wave3Status == WaveStatusReport.end))
            {
                StartWave(Wave3Status, Wave3AmountOfEnemys);
            }
            // Start of the Wave4
            if ((Wave4Status == WaveStatusReport.begin || Wave4Status == WaveStatusReport.running) && !(Wave4Status == WaveStatusReport.end))
            {
                StartWave(Wave4Status, Wave4AmountOfEnemys);
            }
        }
    }

    void StartWave(WaveStatusReport WaveStatus, int AmountOfEnemys)
    {
        if (!(WaveStatus == WaveStatusReport.running))
        {
            for (int i = 0; i < spawner.Length; i++)
            {
                spawner[i].SpawnRate = Random.Range(0.0f, 2.0f);
                spawner[i].AmountToSpawn = AmountOfEnemys / spawner.Length;
                spawner[i].enabled = true;
            }
        }
        Debug.Log("gsdgh");
        WaveStatus = WaveStatusReport.running;
        if (GameManager.Instance.Enemies.Count == 0)
        {
            StartCoroutine(EndOfWave(PauseBetweenWaves));
        }
    }


    private IEnumerator EndOfWave(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (Wave1Status == WaveStatusReport.running && !(Wave1Status == WaveStatusReport.end))
        {
            // Do Something when wave1 is over

            Wave1Status = WaveStatusReport.end;     // End of Wave1
            Wave2Status = WaveStatusReport.begin;   // Start of Wave2
        }
        if (Wave2Status == WaveStatusReport.running && !(Wave2Status == WaveStatusReport.end))
        {
            // Do Something when wave2 is over

            Wave2Status = WaveStatusReport.end;     // End of Wave2
            Wave3Status = WaveStatusReport.begin;   // Start of Wave3
        }
        if (Wave3Status == WaveStatusReport.running && !(Wave3Status == WaveStatusReport.end))
        {
            // Do Something when wave3 is over

            Wave3Status = WaveStatusReport.end;     // End of Wave3
            Wave4Status = WaveStatusReport.begin;   // Start of Wave4
        }
    }


}
