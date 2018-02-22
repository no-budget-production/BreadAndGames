using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[System.Serializable]
//public class WaveArray
//{
//    public WaveStatusReport WaveStatus;
//    public int waveNumber;
//}

[SerializeField]
public enum WaveStatusReport
{
    off,
    begin,
    ready,
    running,
    end, _length, _invalid = -1
}

//[ExecuteInEditMode]
public class PhaseOneLevelScript : MonoBehaviour
{
    //public int amountOfWaves;
    public bool startArenaEvent;

    public int PauseBetweenWaves;

    public int Wave1AmountOfEnemys;
    public int Wave2AmountOfEnemys;
    public int Wave3AmountOfEnemys;
    public int Wave4AmountOfEnemys;

    public ArenaSpawner[] spawner;

    private WaveStatusReport[] WaveStatus = new WaveStatusReport[4];

    //public WaveArray[] currentWave;

    //private string[] WaveNames;
    

    void Start()
    {
        startArenaEvent = false;
        for (int i = 0; i < WaveStatus.Length; i++)
        {
            WaveStatus[i] = WaveStatusReport.off;
        }


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
            WaveStatus[0] = WaveStatusReport.begin;
            startArenaEvent = false;
        }

        if (Application.isPlaying)
        {
            // Start of the Wave1
            if ((WaveStatus[0] == WaveStatusReport.begin || WaveStatus[0] == WaveStatusReport.running) && !(WaveStatus[0] == WaveStatusReport.end))
            {
                StartWave(Wave1AmountOfEnemys, 0);
            }
            // Start of the Wave2
            if ((WaveStatus[1] == WaveStatusReport.begin || WaveStatus[1] == WaveStatusReport.running) && !(WaveStatus[1] == WaveStatusReport.end))
            {
                StartWave(Wave2AmountOfEnemys, 1);
            }
            // Start of the Wave3
            if ((WaveStatus[2] == WaveStatusReport.begin || WaveStatus[2] == WaveStatusReport.running) && !(WaveStatus[2] == WaveStatusReport.end))
            {
                StartWave(Wave3AmountOfEnemys, 2);
            }
            // Start of the Wave4
            if ((WaveStatus[3] == WaveStatusReport.begin || WaveStatus[3] == WaveStatusReport.running) && !(WaveStatus[3] == WaveStatusReport.end))
            {
                StartWave(Wave4AmountOfEnemys, 3);
            }
        }
    }

    void StartWave(int AmountOfEnemys, int WaveArrayNumber)
    {
        if ((this.WaveStatus[WaveArrayNumber] == WaveStatusReport.begin && !(this.WaveStatus[WaveArrayNumber] == WaveStatusReport.running)))
        {
            for (int i = 0; i < spawner.Length; i++)
            {
                spawner[i].SpawnRate = Random.Range(0.0f, 2.0f);
                spawner[i].AmountToSpawn = AmountOfEnemys / spawner.Length;
                spawner[i].enabled = true;
            }
        }
        
        if (GameManager.Instance.Enemies.Count > 0)
        {
            this.WaveStatus[WaveArrayNumber] = WaveStatusReport.running;
        }
        if (GameManager.Instance.Enemies.Count == 0 && (this.WaveStatus[WaveArrayNumber] == WaveStatusReport.running))
        {
            StartCoroutine(EndOfWave(PauseBetweenWaves));
        }
    }


    private IEnumerator EndOfWave(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        // Called only once if all enemys from wave1 are killed
        if (WaveStatus[0] == WaveStatusReport.running && !(WaveStatus[0] == WaveStatusReport.end))
        {
            for (int i = 0; i < spawner.Length; i++)
            {
                spawner[i]._SpawnedEnemys = 0;
            }
            WaveStatus[0] = WaveStatusReport.end;     // End of Wave1
            WaveStatus[1] = WaveStatusReport.begin;   // Start of Wave2
        }
        if (WaveStatus[1] == WaveStatusReport.running && !(WaveStatus[1] == WaveStatusReport.end))
        {
            // Do Something when wave2 is over

            WaveStatus[1] = WaveStatusReport.end;     // End of Wave2
            WaveStatus[2] = WaveStatusReport.begin;   // Start of Wave3
        }
        if (WaveStatus[2] == WaveStatusReport.running && !(WaveStatus[2] == WaveStatusReport.end))
        {
            // Do Something when wave3 is over

            WaveStatus[2] = WaveStatusReport.end;     // End of Wave3
            WaveStatus[3] = WaveStatusReport.begin;   // Start of Wave4
        }
    }


}
