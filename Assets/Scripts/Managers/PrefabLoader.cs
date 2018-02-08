using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoader : MonoBehaviour
{
    private InstanceRef instanceRef;

    public Transform[] PlayerSpawns;
    public GameObject[] Players;


    public GameObject Camera;

    public Transform SphereTriggerHolder;

    public Transform[] SphereTriggers;

    public Transform[] ReinforcmentPoints;

    public GameObject[] SpawnTrigges;

    public static Quaternion InputRotation;

    //Instances

    public int ActivePlayerCount;
    public GameObject[] ActivePlayers;
    public GameObject ActiveCamera;
    public GameObject[] ActiveTriggerSpawns;

    private Transform curHolder;

    public Transform EnemyHolder;
    public Transform SpawnHolder;
    public Transform ClusterHolder;

    public string[] PlayerTags;

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

    void LoadInstancesRed()
    {
        instanceRef = GameObject.FindGameObjectWithTag("InstanceRef").GetComponent<InstanceRef>();
        PlayerSpawns = instanceRef.PlayerSpawns;
        //SpawnTriggerSpawns = instanceRef.SpawnTriggerSpawns;
        SpawnTrigges = instanceRef.SphereTriggers;
        SpawnHolder = instanceRef.SpawnHolder;
        EnemyHolder = instanceRef.EnemyHolder;
        ClusterHolder = instanceRef.ClusterHolder;
        ReinforcmentPoints = instanceRef.ReinforcmentPoints;
    }

    public void LoadPrefabs()
    {
        LoadInstancesRed();

        PlayerSetup();

        SetupCamera();

        LinkInstances();
    }

    void LinkInstances()
    {
        SpawnSphereTriggerSetup();
    }

    GameObject[] SpawnPreFabs(int amount, GameObject[] prefabArray, Transform[] spawnArray, string holder)
    {
        if (holder != null)
        {
            curHolder = SpawnHolderFunction(holder);
        }

        GameObject[] curPrefabs = new GameObject[amount];

        for (int i = 0; i < amount; i++)
        {
            Vector3 curSpawn = spawnArray[i].position;

            curPrefabs[i] = SpawnPreFab(prefabArray[i], spawnArray[i], holder);
        }

        return curPrefabs;
    }

    GameObject SpawnPreFab(GameObject prefab, Transform transform, string holder)
    {
        GameObject curPrefab = Instantiate(prefab, transform.position, transform.rotation);

        if (holder != null)
        {
            curPrefab.transform.SetParent(curHolder);
        }

        return curPrefab;
    }

    void SetupCamera()
    {
        ActiveCamera.transform.rotation = Camera.transform.rotation;

        Transform[] transformPlayers = new Transform[ActivePlayerCount];
        for (int i = 0; i < ActivePlayerCount; i++)
        {
            transformPlayers[i] = ActivePlayers[i].transform;
        }

        CameraController cameraController = ActiveCamera.GetComponent<CameraController>();
        cameraController.Setup(transformPlayers);

        InputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(ActiveCamera.transform.forward, Vector3.up));
        LatePlayerSetup();
    }

    void SetPlayerTags()
    {
        PlayerTags = new string[ActivePlayerCount];
        for (int i = 0; i < ActivePlayerCount; i++)
        {
            PlayerTags[i] = Players[i].gameObject.tag;
        }
    }

    Transform SpawnHolderFunction(string holderString)
    {
        curHolder = new GameObject(holderString).transform;
        return curHolder.transform;
    }

    void SpawnSphereTriggerSetup()
    {
        //SphereTriggerHolder = SpawnHolderFunction("SpawnHolder");

        int tempLength = SpawnTrigges.Length;
        for (int i = 0; i < tempLength; i++)
        {
            SphereTrigger tempSpawnTrigger = SpawnTrigges[i].GetComponent<SphereTrigger>();
            tempSpawnTrigger.Tags = new string[PlayerTags.Length];
            for (int j = 0; j < PlayerTags.Length; j++)
            {
                tempSpawnTrigger.Tags[j] = PlayerTags[j];
            }

            SwarmSpawn tempSwarmSpawn = SpawnTrigges[i].GetComponent<SwarmSpawn>();
            tempSwarmSpawn.SwarmHandler = SpawnHolder;
            tempSwarmSpawn.ReinforcmentPoints = ReinforcmentPoints;

        }
    }

    void PlayerSetup()
    {
        ActivePlayers = SpawnPreFabs(PlayerSpawns.Length, Players, PlayerSpawns, "PlayerHolder");
        ActivePlayerCount = ActivePlayers.Length;
        ActiveCamera = SpawnPreFab(Camera, Camera.transform, null);
        SetPlayerTags();
    }

    void LatePlayerSetup()
    {
        for (int i = 0; i < ActivePlayerCount; i++)
        {
            ActivePlayers[i].GetComponent<PlayerController>().Setup(InputRotation, ButtonStrings, ActivePlayers);
        }
    }
}
