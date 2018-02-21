using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    public List<Enemy> Enemies;


    public Transform EnemyHolder;
    public Transform SpawnHolder;
    public Transform ClusterHolder;
    public Transform TriggerHolder;
    public Transform _DynamicHolder;
    public Transform ProjectileHolder;
    public Transform VisualsHolder;

    public GameObject[] Triggers;
    public Transform[] ReinforcmentPoints;

    public Quaternion InputRotation;

    public PlayerController GetMelee() { return GetPlayerByType(PlayerType.Melee); }
    public PlayerController GetShooter() { return GetPlayerByType(PlayerType.Shooter); }
    public PlayerController GetSupport() { return GetPlayerByType(PlayerType.Support); }
    public PlayerController GetPlayerByType(PlayerType t) { return Players.Where(p => p.Type == t).FirstOrDefault(); }

    public Slider[] HUDHealthBarSlider;
    public Slider[] HUDActionPointsBar;

    void Awake()
    {
        InitGame();
    }

    public void GameOver()
    {
        //Destroy(this);
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
        _DynamicHolder = InstanceRef._DynamicHolder;
        ProjectileHolder = InstanceRef.ProjectileHolder;
        VisualsHolder = InstanceRef.VisualsHolder;

        HUDHealthBarSlider = InstanceRef.HUDHealthBarSlider;
        HUDActionPointsBar = InstanceRef.HUDActionPointsBar;
    }
}