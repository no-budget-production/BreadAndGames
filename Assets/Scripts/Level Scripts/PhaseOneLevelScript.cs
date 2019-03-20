using System.Collections;
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
    public GameObject afterExplosionFire;
    public Transform pointCamera;

    public ArenaSpawner[] spawner;

    public AudioSource AudioSource_Announcer;
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

    public PhaseTwoLevelScript _PhaseTwoLevelScript;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
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

        Invoke("DelayedArenaEvent", 5f);
        //startArenaEvent = true;
    }

    void DelayedArenaEvent()
    {
        startArenaEvent = true;
    }

    void Update()
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
        if (Input.GetKeyDown(KeyCode.D))
        {
            WaveStatus[0] = WaveStatusReport.begin;
        }
        // End Debug


        if (startArenaEvent)
        {
            AudioSource_Announcer.clip = firstCountdown;
            AudioSource_Announcer.Play();
            StartCoroutine(StartEvent(AudioSource_Announcer.clip.length));
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
        AudioSource_Announcer.clip = horn;
        AudioSource_Announcer.Play();
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


        if (gameManager.Enemies.Count > 0)
        {
            this.WaveStatus[WaveArrayNumber] = WaveStatusReport.running;
        }
        if (isFinalWave)
        {
            if (AmountOfSpawnedEnemysInCurrentWave >= phase2TriggerCount)       // begin the end  of phase1 (transition to phase2)
            {
                _PhaseTwoLevelScript.gameObject.SetActive(true);
                EndOfPhase = true;
                StartCoroutine(BreakoutSequenz(3f));        // Parameter = time between change camera and explosion
            }
        }
        if ((AmountOfSpawnedEnemysInCurrentWave >= AmountOfEnemys) && (this.WaveStatus[WaveArrayNumber] == WaveStatusReport.running) && !isFinalWave)
        {
            if (gameManager.Enemies.Count == 0)
            {
                StartCoroutine(EndOfWave(PauseBetweenWaves, WaveArrayNumber));
            }
        }
    }


    private IEnumerator EndOfWave(float waitTime, int WaveArrayNumber)
    {
        if (PlaySound && !(WaveArrayNumber > countdownClips.Length))
        {
            AudioSource_Announcer.clip = countdownClips[WaveArrayNumber];
            AudioSource_Announcer.Play();
            PlaySound = false;
        }

        yield return new WaitForSeconds(AudioSource_Announcer.clip.length);
        WaveEnd(WaveArrayNumber);
        PlaySound = true;
    }

    private IEnumerator BreakoutSequenz(float waitTime)
    {
        for (int i = 0; i < spawner.Length; i++)
        {
            spawner[i].enabled = false;
        }

        GameObject TempObjectHolder1;
        TempObjectHolder1 = Instantiate(explosionPrefab2, explosionPoint.position, explosionPoint.rotation) as GameObject;


        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < GameManager.Instance.Enemies.Count; i++)
        {
            gameManager.Enemies[i].Ebomb();
        }

        gameManager.ActiveCamera.TargetPlayer = new Transform[3];
        gameManager.ActiveCamera.TargetPlayer[2] = pointCamera;
        gameManager.ActiveCamera.TargetPlayer[0] = gameManager.Players[0].GetTransform();
        gameManager.ActiveCamera.TargetPlayer[1] = gameManager.Players[1].GetTransform();

        yield return new WaitForSeconds(waitTime);

        GameObject TempObjectHolder2;
        TempObjectHolder2 = Instantiate(explosionPrefab, explosionPoint.position, explosionPoint.rotation) as GameObject;
        Destroy(roadblock);

        afterExplosionFire.SetActive(true);

        StartCoroutine(CamerBackToNormal(5f));      // Parameter = time till camera gets back to normal
    }
    private IEnumerator CamerBackToNormal(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _PhaseTwoLevelScript.PlayAlarm();
        gameManager.ActiveCamera.TargetPlayer = new Transform[2];
        gameManager.ActiveCamera.TargetPlayer[0] = gameManager.Players[0].GetTransform();
        gameManager.ActiveCamera.TargetPlayer[1] = gameManager.Players[1].GetTransform();

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
                AudioSource_Announcer.clip = horn;
                AudioSource_Announcer.Play();
            }

        }
    }



}
