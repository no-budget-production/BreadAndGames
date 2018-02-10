using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameManagerHelper
{
    public static GameManager gm(this MonoBehaviour s)
    {
        return GameManager.Instance;
    }
}


public class GameManager : MonoBehaviour
{

    public InstanceRef InstanceRef;

    public static GameManager Instance = null;

    public PrefabLoader prefabLoader;

    //Instances

    //public int Players.Length;
    public List<PlayerController> Players;
    public CameraController ActiveCamera;

    public List<Transform> PlayerSpawns;


    public List<SwarmController> Enemies;

    public Transform EnemyHolder;
    public Transform SpawnHolder;
    public Transform ClusterHolder;
    public Transform TriggerHolder;

    public GameObject[] TriggerSpawns;
    public Transform[] Triggers;
    public Transform[] ReinforcmentPoints;
    public GameObject[] SpawnTrigges;


    public Quaternion InputRotation;

    public PlayerController GetMelee() { return GetPlayerByType(PlayerController.PlayerType.Melee); }
    public PlayerController GetShooter() { return GetPlayerByType(PlayerController.PlayerType.Shooter); }
    public PlayerController GetSupport() { return GetPlayerByType(PlayerController.PlayerType.Support); }
    public PlayerController GetPlayerByType(PlayerController.PlayerType t) { return Players.Where(p => p.Type == t).FirstOrDefault(); }


    void Awake()
    {
        InitGame();
    }

    public void GameOver()
    {
        enabled = false;
    }

    void InitGame()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void LoadInstancesRef()
    {
        PlayerSpawns = InstanceRef.PlayerSpawns;
        SpawnTrigges = InstanceRef.SphereTriggers;
        SpawnHolder = InstanceRef.SpawnHolder;
        EnemyHolder = InstanceRef.EnemyHolder;
        ClusterHolder = InstanceRef.ClusterHolder;
        ReinforcmentPoints = InstanceRef.ReinforcementPoints;
    }
}
