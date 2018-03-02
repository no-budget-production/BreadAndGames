using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PrefabLoader : MonoBehaviour
{
    public GameObject[] PlayerPrefabs;
    public GameObject CameraPrefab;

    private Transform curHolder;

    public StatsTracker StatsTracker;

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
        GameManager.Instance.ActiveCamera.transform.rotation = CameraPrefab.transform.rotation;

        Transform[] transformPlayers = new Transform[GameManager.Instance.Players.Count];
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            transformPlayers[i] = GameManager.Instance.Players[i].transform;
        }

        CameraController cameraController = GameManager.Instance.ActiveCamera.GetComponent<CameraController>();
        cameraController.Setup(transformPlayers);

        GameManager.Instance.InputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(GameManager.Instance.ActiveCamera.transform.forward, Vector3.up));
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
        GameManager.Instance.Players = SpawnPreFabs<PlayerController>(PlayerPrefabs, GameManager.Instance.PlayerSpawns, "PlayerHolder");
        GameManager.Instance.ActiveCamera = SpawnPreFab<CameraController>(CameraPrefab, CameraPrefab.transform, null);
    }

    void LatePlayerSetup()
    {
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            GameManager.Instance.Players[i].GetComponent<PlayerController>().Setup(GameManager.Instance.InputRotation, ButtonStrings);

            if (GameManager.Instance.Players[i].UseHUDHealthbarSlider)
            {
                GameManager.Instance.Players[i].HUDHealthBarSlider = GameManager.Instance.HUDHealthBarSlider[i];
                GameManager.Instance.HUDHealthBarSlider[i].maxValue = GameManager.Instance.Players[i].MaxHealth;
                GameManager.Instance.Players[i].OnHUDChangeHealthSlider();
            }
            else
            {
                GameManager.Instance.HUDHealthBarSlider[i].enabled = false;
            }

            if (GameManager.Instance.Players[i].UseHUDActionPointsBar)
            {
                GameManager.Instance.Players[i].HUDActionPointsBar = GameManager.Instance.HUDActionPointsBar[i];
                GameManager.Instance.HUDActionPointsBar[i].maxValue = GameManager.Instance.Players[i].maxActionPoints;
                GameManager.Instance.Players[i].OnActionBarChange();
            }
            else
            {
                GameManager.Instance.HUDActionPointsBar[i].enabled = false;
            }

            if (GameManager.Instance.Players[i].UseHUDActionPointsBar)
            {
                GameManager.Instance.Players[i].OverChargeBar = GameManager.Instance.HUDOverChargeBar[i];
                GameManager.Instance.HUDOverChargeBar[i].maxValue = GameManager.Instance.Players[i].maxOverCharge;
                GameManager.Instance.Players[i].OnChangeOverchargeSlider();
            }
            else
            {
                GameManager.Instance.HUDOverChargeBar[i].enabled = false;
            }

            if (GameManager.Instance.Players[i].UseReloadBar)
            {
                GameManager.Instance.Players[i].ReloadBar = GameManager.Instance.HUDReloadBar[i];
                GameManager.Instance.HUDReloadBar[i].maxValue = (int)(GameManager.Instance.Players[i].maxReloadBar / GameManager.Instance.Players[i].curDisplaySteps);
                GameManager.Instance.Players[i].OnChangeReloadSlider();

            }
            else
            {
                GameManager.Instance.HUDReloadBar[i].enabled = false;
            }

            GameManager.Instance.Players[i].Canvas = GameManager.Instance.HUDCanvas[i];

            GameManager.Instance.Players[i].DamageImage = GameManager.Instance.DamageImage[i];

            GameManager.Instance.Players[i].InternalPlayerNumber = i;
        }
    }

    public void LoadStatsTracker()
    {
        if (StatsTracker.Instance == null)
        {
            StatsTracker curStatsTracker = Instantiate(StatsTracker);

            GameManager.Instance.StatsTracker = curStatsTracker;
        }
        else
        {
            GameManager.Instance.StatsTracker = StatsTracker.Instance;
        }

        StatsTracker.Instance.Kills = new int[GameManager.Instance.Players.Count];
        StatsTracker.Instance.DamageDealt = new float[GameManager.Instance.Players.Count];

        StatsTracker.Instance.RevivedTeamMate = new int[GameManager.Instance.Players.Count];
        StatsTracker.Instance.RevivedSelf = new int[GameManager.Instance.Players.Count];

        StatsTracker.Instance.Downed = new int[GameManager.Instance.Players.Count];
        StatsTracker.Instance.HealthPacks = new int[GameManager.Instance.Players.Count];
        StatsTracker.Instance.Healed = new float[GameManager.Instance.Players.Count];
    }
}