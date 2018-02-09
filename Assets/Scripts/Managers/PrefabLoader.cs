using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrefabLoader : MonoBehaviour
{
    public GameObject[] PlayerPrefabs;
    public GameObject CameraPrefab;

    //public string[] PlayerTags;

    private Transform curHolder;

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

        LinkInstances();
    }

    void LinkInstances()
    {
        SpawnSphereTriggerSetup();
    }

    //List<GameObject> SpawnPreFabs<T>(GameObject[] prefabArray, List<Transform> spawnList, string holder)
    //{
    //    int amount = spawnList.Count;

    //    if (holder != null)
    //    {
    //        curHolder = SpawnHolderFunction(holder);
    //    }

    //    List<GameObject> curPrefabs = new List<GameObject>();

    //    for (int i = 0; i < amount; i++)
    //    {
    //        Debug.Log(i);

    //        Vector3 curSpawn = spawnList[i].position;

    //        GameObject curPrefab = SpawnPreFab<GameObject>(prefabArray[i], spawnList[i], holder);

    //        curPrefabs.Add(curPrefab);
    //    }

    //    return curPrefabs;
    //}

    //GameObject SpawnPreFab<T>(GameObject prefab, Transform transform, string holder)
    //{
    //    Debug.Log("A");

    //    GameObject curPrefab = Instantiate(prefab, transform.position, transform.rotation);

    //    if (holder != null)
    //    {
    //        curPrefab.transform.SetParent(curHolder);
    //    }

    //    return curPrefab;
    //}

    List<T> SpawnPreFabs<T>(GameObject[] prefabArray, List<Transform> spawnList, string holder)
    {
        int amount = spawnList.Count;

        if (holder != null)
        {
            curHolder = SpawnHolderFunction(holder);
        }

        List<T> curPrefabs = new List<T>();

        for (int i = 0; i < amount; i++)
        {
            Debug.Log(i);

            Vector3 curSpawn = spawnList[i].position;

            T curPrefab = SpawnPreFab<T>(prefabArray[i], spawnList[i], holder);

            curPrefabs.Add(curPrefab);
        }

        return curPrefabs;
    }

    T SpawnPreFab<T>(GameObject prefab, Transform transform, string holder)
    {
        Debug.Log("A");

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

    //void SetPlayerTags()
    //{
    //    PlayerTags = new string[GameManager.Instance.Players.Count];
    //    for (int i = 0; i < GameManager.Instance.Players.Count; i++)
    //    {
    //        PlayerTags[i] = PlayerPrefabs[i].gameObject.tag;
    //    }
    //}

    Transform SpawnHolderFunction(string holderString)
    {
        curHolder = new GameObject(holderString).transform;
        return curHolder.transform;
    }

    void SpawnSphereTriggerSetup()
    {
        //SphereTriggerHolder = SpawnHolderFunction("SpawnHolder");

        int tempLength = GameManager.Instance.SpawnTrigges.Length;
        for (int i = 0; i < tempLength; i++)
        {
            Trigger tempSpawnTrigger = GameManager.Instance.SpawnTrigges[i].GetComponent<Trigger>();

            //foreach (SwarmSpawn tempSwarmSpawn in tempSpawnTrigger.SwarmSpawn)
            //{
            //    tempSwarmSpawn.SwarmHandler = GameManager.Instance.SpawnHolder;
            //    tempSwarmSpawn.ReinforcmentPoints = GameManager.Instance.ReinforcmentPoints;
            //}

        }
    }

    void PlayerSetup()
    {
        GameManager.Instance.Players = SpawnPreFabs<PlayerController>(PlayerPrefabs, GameManager.Instance.PlayerSpawns, "PlayerHolder");
        GameManager.Instance.ActiveCamera = SpawnPreFab<CameraController>(CameraPrefab, CameraPrefab.transform, null);

        //List<GameObject> curPlayers = SpawnPreFabs<GameObject>(PlayerPrefabs, GameManager.Instance.PlayerSpawns, "PlayerHolder");
        //foreach (var item in curPlayers)
        //{
        //    GameManager.Instance.Players.Add(item.GetComponent<PlayerController>());
        //}

        //GameObject curCamera = SpawnPreFab<GameObject>(CameraPrefab, CameraPrefab.transform, null);
        //GameManager.Instance.ActiveCamera = curCamera.GetComponent<CameraController>();
    }

    void LatePlayerSetup()
    {
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            GameManager.Instance.Players[i].GetComponent<PlayerController>().Setup(GameManager.Instance.InputRotation, ButtonStrings);
        }
    }
}
