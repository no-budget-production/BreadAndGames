using System.Collections.Generic;
using UnityEngine;

public class PrefabLoader : MonoBehaviour
{
    public GameObject[] PlayerPrefabs;
    public GameObject CameraPrefab;

    private Transform curHolder;

    public StatsTracker StatsTracker;

    private GameManager gameManager;
    private StatsTracker statsTracker;

    public string[] ButtonStrings = new string[]
    {
            "Horizontal_PX",
            "Vertical_PX",
            "HorizontalLook_PX",
            "VerticalLook_PX",
            "Trigger_Left_PX",
            "Trigger_Right_PX",
            "Bumper_Left_PX",
            "Bumper_Right_PX",
            "Pad_Left_PX",
            "Pad_Right_PX",
            "Pad_Up_PX",
            "Pad_Down_PX",
            "Button_A_PX",
            "Button_B_PX",
            "Button_X_PX",
            "Button_Y_PX",
            "Button_Start_PX",
            "Button_Back_PX",
            "Button_RightStick_PX",
            "Button_LeftStick_PX"
    };

    public void LoadPrefabs()
    {
        gameManager = GameManager.Instance;
        statsTracker = StatsTracker.Instance;

        PlayerSetup();

        SetupCamera();

        LoadStatsTracker();
        //LinkInstances();
    }

    //void LinkInstances()
    //{
    //    SpawnSphereTriggerSetup();
    //}

    List<T> SpawnPreFabs<T>(GameObject[] prefabArray, List<Transform> spawnList, string holder)
    {
        int amount = spawnList.Count;

        if (holder != null)
        {
            curHolder = SpawnHolderFunction(holder);
        }

        List<T> curPrefabs = new List<T>(amount);

        for (int i = 0; i < amount; i++)
        {
            Vector3 curSpawn = spawnList[i].position;

            T curPrefab = SpawnPreFab<T>(prefabArray[i], spawnList[i], holder);

            curPrefabs.Add(curPrefab);
        }

        return curPrefabs;
    }

    T SpawnPreFab<T>(GameObject prefab, Transform transform, string holder)
    {

        GameObject curPrefab = Instantiate(prefab, transform.position, transform.rotation);

        if (holder != null)
        {
            curPrefab.transform.SetParent(curHolder);
        }

        return curPrefab.GetComponent<T>();
    }

    void SetupCamera()
    {
        gameManager.ActiveCamera.transform.rotation = CameraPrefab.transform.rotation;

        Transform[] transformPlayers = new Transform[gameManager.Players.Count];
        for (int i = 0; i < gameManager.Players.Count; i++)
        {
            transformPlayers[i] = gameManager.Players[i].transform;
        }

        CameraController cameraController = gameManager.ActiveCamera.GetComponent<CameraController>();
        cameraController.Setup(transformPlayers);

        gameManager.InputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(gameManager.ActiveCamera.transform.forward, Vector3.up));
        LatePlayerSetup();
    }

    Transform SpawnHolderFunction(string holderString)
    {
        curHolder = new GameObject(holderString).transform;
        return curHolder.transform;
    }

    //void SpawnSphereTriggerSetup()
    //{
    //    int tempLength = GameManager.Instance.Triggers.Length;
    //    for (int i = 0; i < tempLength; i++)
    //    {
    //        Trigger tempTrigger = GameManager.Instance.Triggers[i].GetComponent<Trigger>();

    //        for (int j = 0; j < tempTrigger.EnabledWithPlayer.Length; j++)
    //        {
    //            var tempSwarmSpawn = tempTrigger.EnabledWithPlayer[j].GetComponent<EnemySpawn>();
    //            if (tempSwarmSpawn == null)
    //            {
    //                continue;
    //            }
    //            tempSwarmSpawn.EnemyHolder = GameManager.Instance.SpawnHolder;
    //            tempSwarmSpawn.ReinforcmentPoints = GameManager.Instance.ReinforcmentPoints;
    //        }
    //    }
    //}

    void PlayerSetup()
    {
        gameManager.Players = SpawnPreFabs<PlayerController>(PlayerPrefabs, gameManager.PlayerSpawns, "PlayerHolder");
        gameManager.ActiveCamera = SpawnPreFab<CameraController>(CameraPrefab, CameraPrefab.transform, null);
    }

    void LatePlayerSetup()
    {
        for (int i = 0; i < gameManager.Players.Count; i++)
        {
            gameManager.Players[i].GetComponent<PlayerController>().Setup(gameManager.InputRotation, ButtonStrings);

            if (gameManager.Players[i].UseHUDHealthbarSlider)
            {
                gameManager.Players[i].HUDHealthBarSlider = gameManager.HUDHealthBarSlider[i];
                gameManager.Players[i].HUDHealthBarDelay = gameManager.HUDHealthBarDelay[i];
                gameManager.HUDHealthBarSlider[i].maxValue = gameManager.Players[i].MaxHealth;
                gameManager.Players[i].OnHUDChangeHealthSlider();
            }
            else
            {
                gameManager.HUDHealthBarSlider[i].enabled = false;
            }

            if (gameManager.Players[i].UseHUDActionPointsBar)
            {
                gameManager.Players[i].HUDActionPointsBar = gameManager.HUDActionPointsBar[i];
                gameManager.Players[i].HUDActionPointsBarDelay = gameManager.HUDActionPointsBarDelay[i];
                gameManager.HUDActionPointsBar[i].maxValue = gameManager.Players[i].maxActionPoints;
                gameManager.Players[i].OnActionBarChange();
            }
            else
            {
                gameManager.HUDActionPointsBar[i].enabled = false;
            }

            if (gameManager.Players[i].UseHUDActionPointsBar)
            {
                gameManager.Players[i].OverChargeBar = gameManager.HUDOverChargeBar[i];
                gameManager.Players[i].HUDOverChargeBarDelay = gameManager.HUDOverChargeBarDelay[i];
                gameManager.HUDOverChargeBar[i].maxValue = gameManager.Players[i].maxOverCharge;
                gameManager.Players[i].OnChangeOverchargeSlider();
            }
            else
            {
                gameManager.HUDOverChargeBar[i].enabled = false;
            }

            if (gameManager.Players[i].UseReloadBar)
            {
                gameManager.Players[i].ReloadBar = gameManager.HUDReloadBar[i];
                gameManager.Players[i].HUDReloadBarDelay = gameManager.HUDReloadBarDelay[i];
                gameManager.HUDReloadBar[i].maxValue = (int)(gameManager.Players[i].maxReloadBar / gameManager.Players[i].curDisplaySteps);
                gameManager.Players[i].OnChangeReloadSlider();

            }
            else
            {
                gameManager.HUDReloadBar[i].enabled = false;
            }

            if (gameManager.Players[i].UseRealReloadBar)
            {
                gameManager.Players[i].HUDRealReloadBar = gameManager.HUDRealReloadBar[i];
                gameManager.Players[i].HUDRealReloadBarDelay = gameManager.HUDRealReloadBarDelay[i];
                gameManager.HUDRealReloadBar[i].maxValue = gameManager.Players[i].maxReloadBar;
                gameManager.Players[i].OnChangeRealReloadSlider();

            }
            else
            {
                gameManager.HUDRealReloadBar[i].enabled = false;
            }

            gameManager.Players[i].Canvas = gameManager.HUDCanvas[i];

            gameManager.Players[i].DamageImage = gameManager.DamageImage[i];

            gameManager.Players[i].InternalPlayerNumber = i;
        }
    }

    public void LoadStatsTracker()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        if (statsTracker == null)
        {
            statsTracker = StatsTracker.Instance;

            StatsTracker curStatsTracker = Instantiate(StatsTracker);

            gameManager.StatsTracker = curStatsTracker;
            statsTracker = curStatsTracker;
        }
        else
        {
            StatsTracker curStatsTracker = StatsTracker.Instance;
            gameManager.StatsTracker = curStatsTracker;
            statsTracker = curStatsTracker;
        }


        statsTracker.Kills = new int[gameManager.Players.Count];
        statsTracker.DamageDealt = new float[gameManager.Players.Count];

        statsTracker.RevivedTeamMate = new int[gameManager.Players.Count];
        statsTracker.RevivedSelf = new int[gameManager.Players.Count];

        statsTracker.Downed = new int[gameManager.Players.Count];
        statsTracker.HealthPacks = new int[gameManager.Players.Count];
        statsTracker.Healed = new float[gameManager.Players.Count];
    }
}