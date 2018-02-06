using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoader : MonoBehaviour
{
    public Vector3[] PlayerSpawns;
    public GameObject[] Players;
    private Transform playerHolder;

    public GameObject Camera;

    //
    public static Quaternion InputRotation;

    //Instances

    public int ActivePlayerCount;
    public GameObject[] ActivePlayers;
    public GameObject ActiveCamera;

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
        ActivePlayers = SpawnPreFabs(Players, PlayerSpawns, "PlayerHolder");
        ActiveCamera = SpawnPreFab(Camera, Camera.transform.position, null);
        SetupCamera();

    }

    GameObject[] SpawnPreFabs(GameObject[] prefabArray, Vector3[] spawnArray, string holder)
    {
        if (holder != null)
        {
            playerHolder = new GameObject(holder).transform;
        }

        int tempLength = prefabArray.Length;

        GameObject[] curPrefabs = new GameObject[tempLength];

        for (int i = 0; i < tempLength; i++)
        {
            Vector3 curSpawn = spawnArray[i];

            curPrefabs[i] = SpawnPreFab(prefabArray[i], curSpawn, holder);
        }

        return curPrefabs;
    }

    GameObject SpawnPreFab(GameObject prefab, Vector3 location, string holder)
    {
        GameObject curPrefab = Instantiate(prefab, location, Quaternion.identity);

        if (holder != null)
        {
            curPrefab.transform.SetParent(playerHolder);
        }

        return curPrefab;
    }

    void SetupCamera()
    {
        ActiveCamera.transform.rotation = Camera.transform.rotation;

        int tempLength = Players.Length;
        Transform[] transformPlayers = new Transform[tempLength];
        for (int i = 0; i < tempLength; i++)
        {
            transformPlayers[i] = Players[i].transform;
        }

        CameraController tempTeamScript = ActiveCamera.GetComponent<CameraController>();
        tempTeamScript.Setup(transformPlayers);

        InputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(ActiveCamera.transform.forward, Vector3.up));
        LatePlayerSetup();
    }

    void LatePlayerSetup()
    {
        ActivePlayerCount = ActivePlayers.Length;

        for (int i = 0; i < ActivePlayerCount; i++)
        {
            ActivePlayers[i].GetComponent<PlayerController>().Setup(InputRotation, ButtonStrings);
        }
    }
}
