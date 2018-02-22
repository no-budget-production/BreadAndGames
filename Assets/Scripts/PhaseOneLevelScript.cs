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
            if (!(WaveStatus[0] == WaveStatusReport.end) && !(WaveStatus[0] == WaveStatusReport.off))
            {
                StartWave(Wave1AmountOfEnemys, 0);
            }
            // Start of the Wave2
            if (!(WaveStatus[1] == WaveStatusReport.end) && !(WaveStatus[1] == WaveStatusReport.off))
            {
                Debug.Log("dsagsdg");
                StartWave(Wave2AmountOfEnemys, 1);
            }
            // Start of the Wave3
            if (!(WaveStatus[2] == WaveStatusReport.end) && !(WaveStatus[2] == WaveStatusReport.off))
            {
                StartWave(Wave3AmountOfEnemys, 2);
            }
            // Start of the Wave4
            if (!(WaveStatus[3] == WaveStatusReport.end) && !(WaveStatus[3] == WaveStatusReport.off))
            {
                StartWave(Wave4AmountOfEnemys, 3);
            }
        }
    }

    void StartWave(int AmountOfEnemys, int WaveArrayNumber)
    {
        if (!(this.WaveStatus[WaveArrayNumber] == WaveStatusReport.ready) && !(this.WaveStatus[WaveArrayNumber] == WaveStatusReport.running))
        {
            for (int i = 0; i < spawner.Length; i++)
            {
                spawner[i].SpawnRate = Random.Range(0.0f, 2.0f);
                spawner[i].AmountToSpawn = AmountOfEnemys / spawner.Length;
                spawner[i].StartSpawn();
                this.WaveStatus[WaveArrayNumber] = WaveStatusReport.ready;
            }
        }

        
        if (GameManager.Instance.Enemies.Count > 0)
        {
            this.WaveStatus[WaveArrayNumber] = WaveStatusReport.running;
        }
        if (GameManager.Instance.Enemies.Count == 0 && (this.WaveStatus[WaveArrayNumber] == WaveStatusReport.running))
        {
            StartCoroutine(EndOfWave(PauseBetweenWaves, WaveArrayNumber));
        }
    }


    private IEnumerator EndOfWave(float waitTime, int WaveArrayNumber)
    {
        yield return new WaitForSeconds(waitTime);
        WaveEnd(WaveArrayNumber);

    }

    void WaveEnd(int WaveArrayNumber)
    {
        if (WaveStatus[WaveArrayNumber] == WaveStatusReport.running)
        {
            for (int i = 0; i < spawner.Length; i++)
            {
                spawner[i]._SpawnedEnemys = 0;
            }
            WaveStatus[WaveArrayNumber] = WaveStatusReport.end;     // End of Wave

            if (!(WaveArrayNumber + 1 >= WaveStatus.Length))
            {
                WaveStatus[WaveArrayNumber + 1] = WaveStatusReport.begin;   // Start of Nextwave
            }

        }
    }


}
