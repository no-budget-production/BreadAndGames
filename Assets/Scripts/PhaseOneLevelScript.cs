using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum WaveStatusReport
{
    off,
    begin,
    ready,
    running,
    end, _length, _invalid = -1
}

public class PhaseOneLevelScript : MonoBehaviour
{

    private bool startArenaEvent;

    public int PauseBetweenWaves;

    public int Wave1AmountOfEnemys;
    public int Wave2AmountOfEnemys;
    public int Wave3AmountOfEnemys;
    public int Wave4AmountOfEnemys;
    public int WaveFinalAmountOfEnemys;
    public int phase2TriggerCount;

    public GameObject roadblock;
    public GameObject explosionPrefab;
    public GameObject explosionPrefab2;
    public Transform explosionPoint;
    public Transform pointCamera;

    public ArenaSpawner[] spawner;

    private AudioSource AudioSource_Comp;
    public AudioClip firstCountdown;
    public AudioClip[] countdownClips;
    public AudioClip horn;
    private bool PlaySound;

    private WaveStatusReport[] WaveStatus = new WaveStatusReport[5];
    private int WaveCounter;

    private float MinSpawnInterval;
    private float MaxSpawnInterval;
    private bool StartCountdown;

    private bool EndOfPhase;

    private int AmountOfSpawnedEnemysInCurrentWave;
    public int _AmountOfSpawnedEnemysInCurrentWave { get { return AmountOfSpawnedEnemysInCurrentWave; } set { AmountOfSpawnedEnemysInCurrentWave = value; } }


    void Awake()
    {
        AudioSource_Comp = GetComponent<AudioSource>();
    }

    void Start()
    {
        PlaySound = true;
        EndOfPhase = false;
        startArenaEvent = false;
        StartCountdown = true;
        for (int i = 0; i < WaveStatus.Length; i++)
        {
            WaveStatus[i] = WaveStatusReport.off;
        }

        WaveCounter = 0;
        MinSpawnInterval = 5;
        MaxSpawnInterval = 7;
    }

    void Update ()
    {
        // Debug
        if (Input.GetKeyDown(KeyCode.S))
        {
            startArenaEvent = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            WaveStatus[0] = WaveStatusReport.end;
            WaveStatus[1] = WaveStatusReport.end;
            WaveStatus[2] = WaveStatusReport.end;
            WaveStatus[3] = WaveStatusReport.end;
            WaveStatus[4] = WaveStatusReport.begin;
        }
        // End Debug


        if (startArenaEvent)
        {
            AudioSource_Comp.clip = firstCountdown;
            AudioSource_Comp.Play();
            StartCoroutine(StartEvent(AudioSource_Comp.clip.length));            
            startArenaEvent = false;
        }
        else if (!EndOfPhase)
        {
            // Start of the Wave1
            if (!(WaveStatus[0] == WaveStatusReport.end) && !(WaveStatus[0] == WaveStatusReport.off))
            {
                StartWave(Wave1AmountOfEnemys, 0, false);
            }
            // Start of the Wave2
            if (!(WaveStatus[1] == WaveStatusReport.end) && !(WaveStatus[1] == WaveStatusReport.off))
            {
                StartWave(Wave2AmountOfEnemys, 1, false);
            }
            // Start of the Wave3
            if (!(WaveStatus[2] == WaveStatusReport.end) && !(WaveStatus[2] == WaveStatusReport.off))
            {
                StartWave(Wave3AmountOfEnemys, 2, false);
            }
            // Start of the Wave4
            if (!(WaveStatus[3] == WaveStatusReport.end) && !(WaveStatus[3] == WaveStatusReport.off))
            {
                StartWave(Wave4AmountOfEnemys, 3, false);
            }
            // Start of final Wave (transition to phase 2)
            if (!(WaveStatus[4] == WaveStatusReport.end) && !(WaveStatus[4] == WaveStatusReport.off))
            {
                StartWave(WaveFinalAmountOfEnemys, 4, true);
            }
        }
    }

    IEnumerator StartEvent(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        AudioSource_Comp.clip = horn;
        AudioSource_Comp.Play();
        WaveStatus[0] = WaveStatusReport.begin;
    }

    void StartWave(int AmountOfEnemys, int WaveArrayNumber, bool isFinalWave)
    {
        float amountOfEnemysHelper = AmountOfEnemys / spawner.Length;
        if (!(this.WaveStatus[WaveArrayNumber] == WaveStatusReport.ready) && !(this.WaveStatus[WaveArrayNumber] == WaveStatusReport.running))
        {
            if (WaveCounter <= 3 && WaveCounter >= 0)
            {
                MinSpawnInterval--;
                MaxSpawnInterval--;
            }
            for (int i = 0; i < spawner.Length; i++)
            {
                spawner[i]._AmountOfEnemys = amountOfEnemysHelper;
                if (isFinalWave)
                {
                    spawner[i]._Interval = Random.Range(2f, 3f);
                }
                else
                {
                    spawner[i]._Interval = Random.Range(MinSpawnInterval, MaxSpawnInterval);
                }
                spawner[i].enabled = true;
                spawner[i]._StartSpawning = true;
            }
            WaveCounter++;
            this.WaveStatus[WaveArrayNumber] = WaveStatusReport.ready;
        }

        
        if (GameManager.Instance.Enemies.Count > 0)
        {
            this.WaveStatus[WaveArrayNumber] = WaveStatusReport.running;
        }
        if (isFinalWave)
        {
            if (AmountOfSpawnedEnemysInCurrentWave >= phase2TriggerCount)       // Begin of end phase1
            {
                EndOfPhase = true;
                StartCoroutine(BreakoutSequenz(3f));        // Parameter = time between change camera and explosion
            }
        }
        if ((AmountOfSpawnedEnemysInCurrentWave >= AmountOfEnemys) && (this.WaveStatus[WaveArrayNumber] == WaveStatusReport.running) && !isFinalWave)
        {
            if(GameManager.Instance.Enemies.Count == 0)
            {
                StartCoroutine(EndOfWave(PauseBetweenWaves, WaveArrayNumber));
            }
        }
    }


    private IEnumerator EndOfWave(float waitTime, int WaveArrayNumber)
    {
        if (PlaySound && !(WaveArrayNumber > countdownClips.Length))
        {
            AudioSource_Comp.clip = countdownClips[WaveArrayNumber];
            AudioSource_Comp.Play();
            PlaySound = false;
        }

        yield return new WaitForSeconds(AudioSource_Comp.clip.length);
        WaveEnd(WaveArrayNumber);
        PlaySound = true;
    }

    private IEnumerator BreakoutSequenz(float waitTime)
    {
        GameObject TempObjectHolder1;
        TempObjectHolder1 = Instantiate(explosionPrefab2, explosionPoint.position, explosionPoint.rotation) as GameObject;

        yield return new WaitForSeconds(0.5f);

        GameManager.Instance.ActiveCamera.TargetPlayer = new Transform[3];
        GameManager.Instance.ActiveCamera.TargetPlayer[2] = pointCamera;
        GameManager.Instance.ActiveCamera.TargetPlayer[0] = GameManager.Instance.Players[0].GetComponent<Transform>();
        GameManager.Instance.ActiveCamera.TargetPlayer[1] = GameManager.Instance.Players[1].GetComponent<Transform>();

        yield return new WaitForSeconds(waitTime);

        GameObject TempObjectHolder2;
        TempObjectHolder2 = Instantiate(explosionPrefab, explosionPoint.position, explosionPoint.rotation) as GameObject;

        StartCoroutine(CamerBackToNormal(5f));      // Parameter = time till camera gets back to normal
    }
    private IEnumerator CamerBackToNormal(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GameManager.Instance.ActiveCamera.TargetPlayer = new Transform[2];
        GameManager.Instance.ActiveCamera.TargetPlayer[0] = GameManager.Instance.Players[0].GetComponent<Transform>();
        GameManager.Instance.ActiveCamera.TargetPlayer[1] = GameManager.Instance.Players[1].GetComponent<Transform>();

    }

    void WaveEnd(int WaveArrayNumber)
    {
        if (WaveStatus[WaveArrayNumber] == WaveStatusReport.running)
        {
            AmountOfSpawnedEnemysInCurrentWave = 0;
            WaveStatus[WaveArrayNumber] = WaveStatusReport.end;     // End of Wave

            if (!(WaveArrayNumber + 1 >= WaveStatus.Length))
            {
                WaveStatus[WaveArrayNumber + 1] = WaveStatusReport.begin;   // Start of Nextwave
                AudioSource_Comp.clip = horn;
                AudioSource_Comp.Play();
            }

        }
    }



}
