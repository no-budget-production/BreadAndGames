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
    public static GameManager Instance = null;

    public InstanceRef InstanceRef;
    public PrefabLoader prefabLoader;

    //Instances

    public List<PlayerController> Players;
    public CameraController ActiveCamera;

    public List<Transform> PlayerSpawns;
    public List<SwarmController> Enemies;

    public Transform EnemyHolder;
    public Transform SpawnHolder;
    public Transform ClusterHolder;
    public Transform TriggerHolder;

    public GameObject[] Triggers;
    public Transform[] ReinforcmentPoints;

    public Quaternion InputRotation;

    public PlayerController GetMelee() { return GetPlayerByType(PlayerType.Melee); }
    public PlayerController GetShooter() { return GetPlayerByType(PlayerType.Shooter); }
    public PlayerController GetSupport() { return GetPlayerByType(PlayerType.Support); }
    public PlayerController GetPlayerByType(PlayerType t) { return Players.Where(p => p.Type == t).FirstOrDefault(); }

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

        Triggers = InstanceRef.SphereTriggers;

        ReinforcmentPoints = InstanceRef.ReinforcementPoints;

        SpawnHolder = InstanceRef.SpawnHolder;
        EnemyHolder = InstanceRef.EnemyHolder;
        ClusterHolder = InstanceRef.ClusterHolder;
    }
}
